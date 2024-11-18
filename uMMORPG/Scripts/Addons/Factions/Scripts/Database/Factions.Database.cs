#if _SERVER

#if _MYSQL
using MySqlConnector;
using System;
using System.Collections.Generic;

#elif _SQLITE
using SQLite;

#endif

// DATABASE (SQLite / mySQL Hybrid)
public partial class Database
{

#if _SQLITE
	// -----------------------------------------------------------------------------------
    // Character Factions
    // -----------------------------------------------------------------------------------
    class character_factions
    {
        public string character { get; set; }
        public string faction { get; set; }
        public int rating { get; set; }
    }
#endif

    private void Start_Tools_Factions()
    {
        onConnected.AddListener(Connect_Factions);
        onCharacterLoad.AddListener(CharacterLoad_Factions);
        onCharacterSave.AddListener(CharacterSave_Factions);
    }
    // -----------------------------------------------------------------------------------
    // Connect_Factions
    // -----------------------------------------------------------------------------------
    private void Connect_Factions()
    {
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS character_factions (
        				`character` VARCHAR(32) NOT NULL,
        				faction VARCHAR(32)  NOT NULL,
        				rating INTEGER(15),
                        primary key(`character`, faction)
        				) CHARACTER SET=utf8mb4 ");
#elif _SQLITE
        connection.CreateTable<character_factions>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_Factions
    // -----------------------------------------------------------------------------------
    private void CharacterLoad_Factions(Player player)
    {
        if (player.playerFactions)
        {
#if _MYSQL
            var table = MySqlHelper.ExecuteReader(connectionString, "SELECT faction, rating FROM character_factions WHERE `character`=@character", new MySqlParameter("@character", player.name));

            foreach (var row in table)
            {
                Faction faction = new Faction
                {
                    name = (string)row[0],
                    rating = Convert.ToInt32(row[1])
                };
                player.playerFactions.Factions.Add(faction);
            }
#elif _SQLITE
            var table = connection.Query<character_factions>("SELECT faction, rating FROM character_factions WHERE character=?", player.name);

            foreach (var row in table)
            {
                Faction faction = new Faction();
                faction.name = row.faction;
                faction.rating = row.rating;
                player.playerFactions.Factions.Add(faction);
            }
#endif
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_Factions
    // -----------------------------------------------------------------------------------
    private void CharacterSave_Factions(Player player)
    {
        if (player.playerFactions)
        {
#if _MYSQL
            /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_factions WHERE `character` = @character", new MySqlParameter("@character", player.name));
            foreach (Faction faction in player.playerFactions.Factions)
                MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_factions VALUES (@character, @faction, @rating)",
                                new MySqlParameter("@character", player.name),
                                new MySqlParameter("@faction", faction.name),
                                new MySqlParameter("@rating", faction.rating));*/
            // Supprimer les entrées existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_factions WHERE `character` = @character", new MySqlParameter("@character", player.name));

            // Construction de la requête INSERT groupée
            string insertQuery = "INSERT INTO character_factions (`character`, `faction`, `rating`) VALUES ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;

            foreach (Faction faction in player.playerFactions.Factions)
            {
                if (index > 0)
                    insertQuery += ",";

                insertQuery += $"(@character_{index}, @faction_{index}, @rating_{index})";

                parameters.Add(new MySqlParameter($"@character_{index}", player.name));
                parameters.Add(new MySqlParameter($"@faction_{index}", faction.name));
                parameters.Add(new MySqlParameter($"@rating_{index}", faction.rating));

                index++;
            }

            // Exécution de la requête si des paramètres ont été ajoutés
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
            }

#elif _SQLITE
            connection.Execute("DELETE FROM character_factions WHERE character=?", player.name);
            foreach (Faction faction in player.playerFactions.Factions)
                connection.Insert(new character_factions
                {
                    character = player.name,
                    faction = faction.name,
                    rating = faction.rating
                });
#endif
        }
    }

    // -----------------------------------------------------------------------------------
}
#endif

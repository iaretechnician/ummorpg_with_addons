#if _SERVER

#if _MYSQL
using MySqlConnector;
using System.Collections.Generic;

#elif _SQLITE
using SQLite;

#endif

// DATABASE (SQLite / mySQL Hybrid)
public partial class Database
{

#if _SQLITE
	// -----------------------------------------------------------------------------------
    // Character Exploration
    // -----------------------------------------------------------------------------------
    class character_exploration
    {
        public string character { get; set; }
        public string exploredArea { get; set; }
    }
#endif
    private void Start_Tools_Exploration()
    {
        onConnected.AddListener(Connect_Exploration);
        onCharacterLoad.AddListener(CharacterLoad_Exploration);
        onCharacterSave.AddListener(CharacterSave_Exploration);
    }
    // -----------------------------------------------------------------------------------
    // Connect_Exploration
    // -----------------------------------------------------------------------------------
    private void Connect_Exploration()
    {
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS character_exploration (`character` VARCHAR(32) NOT NULL, exploredArea VARCHAR(32) NOT NULL) CHARACTER SET=utf8mb4");
#elif _SQLITE && _SERVER
        connection.CreateTable<character_exploration>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_Exploration
    // -----------------------------------------------------------------------------------
    private void CharacterLoad_Exploration(Player player)
    {
#if _MYSQL
		var table = MySqlHelper.ExecuteReader(connectionString, "SELECT exploredArea FROM character_exploration WHERE `character`=@character",
						new MySqlParameter("@character", player.name)
						);
		foreach (var row in table) {
			player.playerAddonsConfigurator.exploredAreas.Add((string)row[0]);
		}
#elif _SQLITE && _SERVER
        var table = connection.Query<character_exploration>("SELECT exploredArea FROM character_exploration WHERE character=?", player.name);
        foreach (var row in table)
        {
            player.playerAddonsConfigurator.exploredAreas.Add(row.exploredArea);
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_Exploration
    // -----------------------------------------------------------------------------------
    private void CharacterSave_Exploration(Player player)
    {
#if _MYSQL
        /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_exploration WHERE `character`=@character", new MySqlParameter("@character", player.name));
        for (int i = 0; i < player.playerAddonsConfigurator.exploredAreas.Count; ++i)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_exploration VALUES (@character, @exploredArea)",
                 new MySqlParameter("@character", player.name),
                 new MySqlParameter("@exploredArea", player.playerAddonsConfigurator.exploredAreas[i])
                 );
        }*/

        // Suppression des anciennes entrées pour ce personnage dans character_exploration
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_exploration WHERE `character`=@character", new MySqlParameter("@character", player.name));

        // Construction de la requête INSERT groupée pour character_exploration
        string insertExplorationQuery = "INSERT INTO character_exploration (`character`, `exploredArea`) VALUES ";
        List<MySqlParameter> explorationParameters = new List<MySqlParameter>();

        for (int i = 0; i < player.playerAddonsConfigurator.exploredAreas.Count; i++)
        {
            if (i > 0)
                insertExplorationQuery += ",";

            insertExplorationQuery += "(@character_" + i + ", @exploredArea_" + i + ")";
            explorationParameters.Add(new MySqlParameter("@character_" + i, player.name));
            explorationParameters.Add(new MySqlParameter("@exploredArea_" + i, player.playerAddonsConfigurator.exploredAreas[i]));
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (explorationParameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertExplorationQuery, explorationParameters.ToArray());
        }

#elif _SQLITE
        connection.Execute("DELETE FROM character_exploration WHERE character=?", player.name);
        for (int i = 0; i < player.playerAddonsConfigurator.exploredAreas.Count; i++)
        {
            connection.Insert(new character_exploration
            {
                character = player.name,
                exploredArea = player.playerAddonsConfigurator.exploredAreas[i]
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
#endif
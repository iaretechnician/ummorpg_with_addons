#if _SERVER

#if _MYSQL
using MySqlConnector;
using System.Collections.Generic;
using UnityEngine;

#elif _SQLITE
using SQLite;

#endif

// DATABASE (SQLite / mySQL Hybrid)
public partial class Database
{

#if _SQLITE
    // -----------------------------------------------------------------------------------
    // character_traits
    // -----------------------------------------------------------------------------------
    class character_traits
    {
        public string character { get; set; }
        public string name { get; set; }
    }
#endif
    private void Start_Tools_Traits()
    {
        onConnected.AddListener(Connect_Traits);
        onCharacterLoad.AddListener(CharacterLoad_Traits);
        onCharacterSave.AddListener(CharacterSave_Traits);
    }
    // -----------------------------------------------------------------------------------
    // Connect_Traits
    // -----------------------------------------------------------------------------------
    private void Connect_Traits()
    {
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS character_traits (`character` VARCHAR(32) NOT NULL, name VARCHAR(32) NOT NULL)");
#elif _SQLITE
        connection.CreateTable<character_traits>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_Traits
    // -----------------------------------------------------------------------------------
    private void CharacterLoad_Traits(Player player)
    {
#if _MYSQL
		var table = MySqlHelper.ExecuteReader(connectionString, "SELECT name FROM character_traits WHERE `character`=@character", new MySqlParameter("@character", player.name));
#elif _SQLITE
        var table = connection.Query<character_traits>("SELECT name FROM character_traits WHERE character=?", player.name);
#endif
        foreach (var row in table)
        {
#if _MYSQL
            TraitTemplate tmpl = TraitTemplate.dict[((string)row[0]).GetStableHashCode()];
#elif _SQLITE
            TraitTemplate tmpl = TraitTemplate.dict[row.name.GetStableHashCode()];
#endif
            player.playerTraits.Traits.Add(new Trait(tmpl));
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_Traits
    // -----------------------------------------------------------------------------------
    private void CharacterSave_Traits(Player player)
    {
#if _MYSQL
        /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_traits WHERE `character`=@character", new MySqlParameter("@character", player.name));
        for (int i = 0; i < player.playerTraits.Traits.Count; ++i)
        {
            Trait trait = player.playerTraits.Traits[i];
            MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_traits VALUES (@character, @name)",
                    new MySqlParameter("@character", player.name),
                    new MySqlParameter("@name", trait.name)
                    );
        }*/

        // Supprimer les entrées existantes pour ce personnage
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_traits WHERE `character`=@character", new MySqlParameter("@character", player.name));

        // Construction de la requête INSERT groupée
        string insertQuery = "INSERT INTO character_traits (`character`, `name`) VALUES ";

        List<MySqlParameter> parameters = new List<MySqlParameter>();
        int index = 0;

        for (int i = 0; i < player.playerTraits.Traits.Count; ++i)
        {
            if (index > 0)
                insertQuery += ",";

            insertQuery += $"(@character_{index}, @name_{index})";

            parameters.Add(new MySqlParameter($"@character_{index}", player.name));
            parameters.Add(new MySqlParameter($"@name_{index}", player.playerTraits.Traits[i].name));

            index++;
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

#elif _SQLITE
        connection.Execute("DELETE FROM character_traits WHERE character=?", player.name);
        for (int i = 0; i < player.playerTraits.Traits.Count; ++i)
        {
            Trait trait = player.playerTraits.Traits[i];
            connection.Insert(new character_traits
            {
                character = player.name,
                name = trait.name
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
#endif
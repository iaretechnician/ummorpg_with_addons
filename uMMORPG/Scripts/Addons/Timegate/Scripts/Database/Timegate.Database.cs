#if _SERVER
using System;
using System.Collections.Generic;


#if _MYSQL
using MySqlConnector;
#elif _SQLITE
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE
	// -----------------------------------------------------------------------------------
    // Character Timegates
    // -----------------------------------------------------------------------------------
    class character_timegates
    {
        public string character { get; set; }
        public string timegateName { get; set; }
        public int timegateCount { get; set; }
        public string timegateHours { get; set; }
    }
#endif
    private void Start_Tools_SimpleTimegate()
    {
        onConnected.AddListener(Connect_SimpleTimegate);
        onCharacterLoad.AddListener(CharacterLoad_SimpleTimegate);
        onCharacterSave.AddListener(CharacterSave_SimpleTimegate);
    }
    // -----------------------------------------------------------------------------------
    // Connect_SimpleTimegate
    // -----------------------------------------------------------------------------------
    private void Connect_SimpleTimegate()
    {
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS character_timegates (
			`character` VARCHAR(32) NOT NULL,
			timegateName TEXT NOT NULL,
			timegateCount INTEGER NOT NULL,
			timegateHours TEXT NOT NULL
              ) CHARACTER SET=utf8mb4");
#elif _SQLITE
        connection.CreateTable<character_timegates>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_SimpleTimegate
    // -----------------------------------------------------------------------------------
    private void CharacterLoad_SimpleTimegate(Player player)
    {
        if (player.playerAddonsConfigurator)
        {
            player.playerAddonsConfigurator.timegates.Clear();

#if _MYSQL
            var table = MySqlHelper.ExecuteReader(connectionString, "SELECT timegateName, timegateCount, timegateHours FROM character_timegates WHERE `character`=@name", new MySqlParameter("@name", player.name));
            foreach (var row in table)
            {
                Timegate timegate = new Timegate();
                timegate.name = (string)row[0];
                timegate.count = Convert.ToInt32((int)row[1]);
                timegate.hours = (string)row[2];
                timegate.valid = true;
                player.playerAddonsConfigurator.timegates.Add(timegate);
            }
#elif _SQLITE
        var table = connection.Query<character_timegates>("SELECT timegateName, timegateCount, timegateHours FROM character_timegates WHERE character=?", player.name);
        foreach (var row in table)
        {
            Timegate timegate = new Timegate();
            timegate.name = row.timegateName;
            timegate.count = row.timegateCount;
            timegate.hours = row.timegateHours;
            timegate.valid = true;
            player.playerAddonsConfigurator.timegates.Add(timegate);
        }
#endif
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_SimpleTimegate
    // -----------------------------------------------------------------------------------
    private void CharacterSave_SimpleTimegate(Player player)
    {
        if (player.playerAddonsConfigurator)
        {
#if _MYSQL
            /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_timegates WHERE `character`=@character", new MySqlParameter("@character", player.name));
            for (int i = 0; i < player.playerAddonsConfigurator.timegates.Count; ++i)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_timegates VALUES (@character, @timegateName, @timegateCount, @timegateHours)",
                     new MySqlParameter("@character", player.name),
                     new MySqlParameter("@timegateName", player.playerAddonsConfigurator.timegates[i].name),
                     new MySqlParameter("@timegateCount", player.playerAddonsConfigurator.timegates[i].count),
                     new MySqlParameter("@timegateHours", player.playerAddonsConfigurator.timegates[i].hours));
            }*/
            // Supprimer les entrées existantes pour ce personnage
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_timegates WHERE `character`=@character", new MySqlParameter("@character", player.name));

            // Construction de la requête INSERT groupée
            string insertQuery = "INSERT INTO character_timegates (`character`, `timegateName`, `timegateCount`, `timegateHours`) VALUES ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;

            for (int i = 0; i < player.playerAddonsConfigurator.timegates.Count; ++i)
            {
                if (index > 0)
                    insertQuery += ",";

                insertQuery += $"(@character_{index}, @timegateName_{index}, @timegateCount_{index}, @timegateHours_{index})";

                parameters.Add(new MySqlParameter($"@character_{index}", player.name));
                parameters.Add(new MySqlParameter($"@timegateName_{index}", player.playerAddonsConfigurator.timegates[i].name));
                parameters.Add(new MySqlParameter($"@timegateCount_{index}", player.playerAddonsConfigurator.timegates[i].count));
                parameters.Add(new MySqlParameter($"@timegateHours_{index}", player.playerAddonsConfigurator.timegates[i].hours));

                index++;
            }

            // Exécution de la requête si des paramètres ont été ajoutés
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
            }

#elif _SQLITE
        connection.Execute("DELETE FROM character_timegates WHERE character=?", player.name);
        for (int i = 0; i < player.playerAddonsConfigurator.timegates.Count; ++i)
        {
            connection.Insert(new character_timegates
            {
                character = player.name,
                timegateName = player.playerAddonsConfigurator.timegates[i].name,
                timegateCount = player.playerAddonsConfigurator.timegates[i].count,
                timegateHours = player.playerAddonsConfigurator.timegates[i].hours
            });
        }
#endif
        }
    }
    // -----------------------------------------------------------------------------------
}
#endif
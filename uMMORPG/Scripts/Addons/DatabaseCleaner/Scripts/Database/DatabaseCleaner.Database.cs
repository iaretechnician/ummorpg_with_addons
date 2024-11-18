#if _SERVER

using System;
using UnityEngine;

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
    // Account Last Online
    // -----------------------------------------------------------------------------------
    class account_lastonline
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string account { get; set; }
        public string lastOnline { get; set; }
    }
#endif
    private void Start_Tools_DatabaseCleaner()
    {
        onConnected.AddListener(Connect_DatabaseCleaner);
    }
    // -----------------------------------------------------------------------------------
    // Connect_DatabaseCleaner
    // -----------------------------------------------------------------------------------
    private void Connect_DatabaseCleaner()
    {
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS account_lastonline (account VARCHAR(32) NOT NULL, lastOnline VARCHAR(64) NOT NULL, PRIMARY KEY(`account`))");
#elif _SQLITE
        connection.CreateTable<account_lastonline>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // DatabaseCleanerAccountLastOnline
    // -----------------------------------------------------------------------------------
    public void DatabaseCleanerAccountLastOnline(string accountName)
    {
        if (string.IsNullOrWhiteSpace(accountName)) return;
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM account_lastonline WHERE account=@name", new MySqlParameter("@name", accountName));
        MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO account_lastonline VALUES (@account, @lastOnline)",
			new MySqlParameter("@lastOnline", DateTime.UtcNow.ToString("s")),
			new MySqlParameter("@account", accountName));
#elif _SQLITE
        connection.Execute("DELETE FROM account_lastonline WHERE account=?", accountName);
        connection.Insert(new account_lastonline
        {
            account = accountName,
            lastOnline = DateTime.UtcNow.ToString("s")
        });
#endif
    }

    public void Cleanup(Tmpl_DatabaseCleaner databaseCleaner)
    {
        if (databaseCleaner && databaseCleaner.isActive)
        {
            var i = 0;
            // ---------- Prune outdated accounts
            if (databaseCleaner.PruneInactiveAfterDays > 0 || databaseCleaner.PruneBannedAfterDays > 0)
            {
#if _MYSQL
                //var table = ExecuteReaderMySql("SELECT account, lastOnline FROM account_lastonline");
                var table = MySqlHelper.ExecuteReader(connectionString, "SELECT name, lastLogin, banned FROM accounts");
#elif _SQLITE
                var table = connection.Query<accounts>("SELECT name, lastLogin, banned FROM accounts");
#endif

                foreach (var row in table)
                {
#if _MYSQL
                    var accountName = (string)row[0];
                    var lastOnline = (string)row[1].ToString();
                    bool banned = (bool)row[2];

#elif _SQLITE
                    var accountName = row.name;
                    var lastOnline = row.lastlogin.ToString();
                    bool banned = (bool)row.banned;
#endif

                    if (!string.IsNullOrWhiteSpace(accountName))
                    {
                        DateTime time = DateTime.Parse(lastOnline);
                        var HoursPassed = (DateTime.UtcNow - time).TotalDays;
                        // ---------- Prune outdated accounts
                        if (databaseCleaner.PruneInactiveAfterDays > 0 && HoursPassed > databaseCleaner.PruneInactiveAfterDays)
                        {
                            DatabaseCleanup(databaseCleaner, accountName);
                            i++;
                        }

                        // ---------- Prune banned accounts
                        if (databaseCleaner.PruneBannedAfterDays > 0 && HoursPassed > databaseCleaner.PruneBannedAfterDays)
                        {
                            if (banned)
                            {
                                DatabaseCleanup(databaseCleaner, accountName);
                                i++;
                            }
                        }


                        // ---------- Prune empty accounts (no characters)
                        if (databaseCleaner.PruneEmptyAccounts && databaseCleaner.PruneeEmptyAccountsAfterDays > 0 && HoursPassed > databaseCleaner.PruneeEmptyAccountsAfterDays)
                        {
                            var accountChars = CharactersForAccount(accountName);
                            if (accountChars.Count < 1)
                            {
                                DatabaseCleanup(databaseCleaner, accountName);
                                i++;
                            }
                        }
                    }
                }
            }
#if UNITY_EDITOR
            Debug.Log("DatabaseCleaner checking accounts ...pruned [" + i + "] account(s)");
#else
            Console.WriteLine("DatabaseCleaner checking accounts ...pruned [" + i + "] account(s)");
#endif
        }
        else
        {
            Debug.LogWarning("DatabaseCleaner: Either inactive or ScriptableObject not found!");
        }
    }

    // -----------------------------------------------------------------------------------
    // DatabaseCleanup
    // -----------------------------------------------------------------------------------
    public void DatabaseCleanup(Tmpl_DatabaseCleaner databaseCleaner, string accountName)
    {
        var accountChars = CharactersForAccount(accountName);

#if _MYSQL
		foreach (string accountChar in accountChars) {
			foreach (string charTable in databaseCleaner.characterTables) {
				if (charTable != "") {
					if (0 < (long)MySqlHelper.ExecuteScalar(connectionString, "SELECT count(*) FROM "+charTable))
                        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM " +charTable+" WHERE `character`=@name", new MySqlParameter("@name", accountChar));
                    else
                        Debug.LogWarning(" Warning : in " + databaseCleaner + "table \"" + charTable + "\" does not exist !");
				}
			}

			foreach (string accountTable in databaseCleaner.accountTables) {
				if (accountTable != "") {
					if (0 < (long)MySqlHelper.ExecuteScalar(connectionString, "SELECT count(*) FROM " + accountTable))
                        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM " +accountTable+ " WHERE account=@name", new MySqlParameter("@name", accountName));
                    else
                        Debug.LogWarning(" Warning : in " + databaseCleaner + "table \""+ accountTable + "\" does not exist !");

				}
			}
		}
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM characters WHERE account=@name", new MySqlParameter("@name", accountName));
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM account_lastonline WHERE account=@name", new MySqlParameter("@name", accountName));
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM accounts WHERE name=@name", new MySqlParameter("@name", accountName));

#elif _SQLITE
        foreach (string accountChar in accountChars)
        {
            foreach (string charTable in databaseCleaner.characterTables)
            {
                if (charTable != "")
                {
                    var compare = connection.GetTableInfo(charTable);
                    Debug.Log(" table :" + charTable + " = " + compare);
                    if (compare.Count > 0)
                        connection.Execute("DELETE FROM " + charTable + " WHERE character=?", accountChar);
                    else
                        Debug.LogWarning(" Warning : in " + databaseCleaner + "table \"" + accountChar + "\" does not exist !");
                }
            }

            foreach (string accountTable in databaseCleaner.accountTables)
            {
                if (accountTable != "")
                {
                    var compare = connection.GetTableInfo(accountTable);
                    Debug.Log(" table :" + accountTable + " = " + compare);
                    if (compare.Count > 0)
                        connection.Execute("DELETE FROM " + accountTable + " WHERE account=?", accountName);
                    else
                        Debug.LogWarning(" Warning : in " + databaseCleaner + "table \""+ accountName + "\" does not exist!");
                }
            }
        }
        connection.Execute("DELETE FROM characters WHERE account=?", accountName);
        connection.Execute("DELETE FROM account_lastonline WHERE account=?", accountName);
        connection.Execute("DELETE FROM accounts WHERE name=?", accountName);
#endif

        Debug.Log("DatabaseCleaner deleted characters of account [" + accountName + "] and all associated tables.");
    }


    // -----------------------------------------------------------------------------------
}
#endif
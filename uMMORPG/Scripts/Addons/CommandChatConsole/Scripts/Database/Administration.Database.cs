#if _SERVER
#if _MYSQL
using MySqlConnector;
#elif _SQLITE
using SQLite;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{
    protected long accountCount = -1;
#if _SQLITE
	// -----------------------------------------------------------------------------------
    // Account Admin
    // -----------------------------------------------------------------------------------
    class account_admin
    {
        [PrimaryKey] // important for performance: O(log n) instead of O(n)
        public string account { get; set; }
        public int admin { get; set; }
    }
#endif
    private void Start_Tools_Administration()
    {
        onConnected.AddListener(Connect_Administration);
        onCharacterLoad.AddListener(CharacterLoad_Administration);
    }
    // -----------------------------------------------------------------------------------
    // Connect_Administration
    // -----------------------------------------------------------------------------------
    private void Connect_Administration()
    {
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, @"
                        CREATE TABLE IF NOT EXISTS account_admin (
					    `account` VARCHAR(32) NOT NULL,
                        admin INTEGER(4) NOT NULL DEFAULT 0,
                            PRIMARY KEY(`account`)
                        ) CHARACTER SET=utf8mb4");
#elif _SQLITE
        connection.CreateTable<account_admin>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_Administration
    // -----------------------------------------------------------------------------------
    private void CharacterLoad_Administration(Player player)
    {
#if _MYSQL
		var table = MySqlHelper.ExecuteReader(connectionString, "SELECT admin FROM account_admin WHERE `account`=@account", new MySqlParameter("@account", player.account));
		if (table.Count == 1) {
            var row = table[0];
            player.AdminLevel = (int)(row[0]);
        }
#elif _SQLITE
        var table = connection.Query<account_admin>("SELECT admin FROM account_admin WHERE account=?", player.account);
        if (table.Count == 1)
        {
            var row = table[0];
            player.AdminLevel = row.admin;
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // IsBannedAccount
    // -----------------------------------------------------------------------------------
    public bool IsBannedAccount(string account, string password)
    {
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password)) return false;
#if _MYSQL
		var table = MySqlHelper.ExecuteReader(connectionString, "SELECT password, banned FROM accounts WHERE name=@name", new MySqlParameter("@name", account));
#elif _SQLITE
        var table = connection.Query<accounts>("SELECT password, banned FROM accounts WHERE name=?", account);
#endif
        if (table.Count == 1)
        {
            // account exists. check password and ban status.
            var row = table[0];
#if _MYSQL
            return (long)row[1] == 1;
#elif _SQLITE
            return row.banned;
#endif
        }
        return false;
    }

    // -----------------------------------------------------------------------------------
    // GetAccountName
    // -----------------------------------------------------------------------------------
    public string GetAccountName(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName)) return "";
#if _MYSQL
		var table = MySqlHelper.ExecuteReader(connectionString, "SELECT account FROM characters WHERE name=@name", new MySqlParameter("@name", playerName));
#elif _SQLITE
        var table = connection.Query<characters>("SELECT account FROM characters WHERE name=?", playerName);
#endif
        if (table.Count == 1)
        {
            var row = table[0];
#if _MYSQL
            return (string)row[0];
#elif _SQLITE
            return row.account;
#endif
        }
        return "";
    }

    // -----------------------------------------------------------------------------------
    // GetAccountCount
    // -----------------------------------------------------------------------------------
    public long GetAccountCount()
    {
#if _MYSQL
		return (long)MySqlHelper.ExecuteScalar(connectionString, "SELECT count(*) FROM accounts");
#elif _SQLITE
        var results = connection.Query<accounts>("SELECT count(*) FROM accounts");
        return results.Count;
#else
        return 0;
#endif
    }

    // -----------------------------------------------------------------------------------
    // BanAccount
    // -----------------------------------------------------------------------------------
    public void BanAccount(string account)
    {
        if (string.IsNullOrWhiteSpace(account)) return;
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE accounts SET banned = '1' WHERE name =@name", new MySqlParameter("@name", account));
#elif _SQLITE
        connection.Execute("UPDATE accounts SET banned=true WHERE name =?", account);
#endif
    }

    // -----------------------------------------------------------------------------------
    // UnbanAccount
    // -----------------------------------------------------------------------------------
    public void UnbanAccount(string account)
    {
        if (string.IsNullOrWhiteSpace(account)) return;
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE accounts SET banned = '0' WHERE name =@name", new MySqlParameter("@name", account));
#elif _SQLITE
        connection.Execute("UPDATE accounts SET banned=false WHERE name =?", account);
#endif
    }

    // -----------------------------------------------------------------------------------
    // SetAdminAccount
    // -----------------------------------------------------------------------------------
    public void SetAdminAccount(string accountName, int adminLevel)
    {
        if (string.IsNullOrWhiteSpace(accountName)) return;
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, "REPLACE account_admin VALUES (@account,@admin)",
        	new MySqlParameter("@account", accountName),
        	new MySqlParameter("@admin", adminLevel));
#elif _SQLITE
        connection.InsertOrReplace(new account_admin
        {
            account = accountName,
            admin = adminLevel
        });
#endif
    }

    // -----------------------------------------------------------------------------------
    // SetCharacterDeleted
    // -----------------------------------------------------------------------------------
    public void SetCharacterDeleted(string playerName, bool deleted)
    {
        if (string.IsNullOrWhiteSpace(playerName)) return;
#if _MYSQL
        int del = (deleted == true ? 1 : 0);
        MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE accounts SET deleted = '@deleted' WHERE name =@name",
        	new MySqlParameter("@deleted", del),
        	new MySqlParameter("@name", playerName));
#elif _SQLITE 
        connection.Execute("UPDATE accounts SET deleted=? WHERE name =?", deleted, playerName);
#endif
    }

    // -----------------------------------------------------------------------------------
}
#endif
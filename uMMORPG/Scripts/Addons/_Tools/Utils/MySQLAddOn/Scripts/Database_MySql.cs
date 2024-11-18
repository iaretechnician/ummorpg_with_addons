#if _MYSQL

using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using MySqlConnector;
using UnityEngine.Events;

// Database (mySQL)
public partial class Database : MonoBehaviour
{
    public static Database singleton;
    private string connectionString = null;
#if _SERVER || UNITY_EDITOR
    [Tooltip("The first time you launch your server this option must be enabled in order to create the tables on your server, this can remain enabled and this will keep the original behavior which tries to create each table when the server starts, if you uncheck it the attempt to create the tables will not be made the next time the server starts.")]
    public bool createTableOnConnect = true;
    [Tooltip("Be careful if you disable this option, accounts that do not exist will not be able to connect, because they will not be created automatically!")]
    public bool allowAutoRegister = true;

#endif

    [Header("Events")]
    // use onConnected to create an extra table for your addon
    public UnityEvent onConnected;
    public UnityEventPlayer onCharacterLoad;
    public UnityEventPlayer onCharacterSave;

#if _SERVER
    // Méthode d'initialisation de la chaîne de connexion
    public void InitializeConnectionString()
    {
        if (connectionString == null)
        {
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = string.IsNullOrWhiteSpace(dbHost) ? "127.0.0.1" : dbHost,
                Database = string.IsNullOrWhiteSpace(dbName) ? "database" : dbName,
                UserID = string.IsNullOrWhiteSpace(dbUser) ? "user" : dbUser,
                Password = string.IsNullOrWhiteSpace(dbPassword) ? "password" : dbPassword,
                Port = dbPort,
                CharacterSet = string.IsNullOrWhiteSpace(dbCharacterSet) ? "utf8mb4" : dbCharacterSet
            };
            connectionString = connectionStringBuilder.ConnectionString;
        }
    }
#endif

    //private string ConnectionString => connectionString;

    // -----------------------------------------------------------------------------------
    // Awake
    // -----------------------------------------------------------------------------------
#if _SERVER
    void Awake()
    {
        if (singleton == null) singleton = this;

        InitializeConnectionString();
        Utils.InvokeMany(typeof(Database), this, "Start_Tools_");
    }
#endif
    // -----------------------------------------------------------------------------------
    // Connect
    // -----------------------------------------------------------------------------------

    public void Connect()
    {
#if _SERVER
        if (createTableOnConnect)
        {
            // -- accounts
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS accounts (
            name VARCHAR(32) NOT NULL,
            password VARCHAR(64) NOT NULL,
            created DATETIME NOT NULL,
            lastlogin DATETIME NOT NULL,
            online BOOLEAN NOT NULL DEFAULT 0,
            banned BOOLEAN NOT NULL DEFAULT 0,
            PRIMARY KEY(name)
        )  ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"); // convert to innobd

            // -- characters
#if !_iMMO2D
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS characters(
            name VARCHAR(32) NOT NULL,
            account VARCHAR(32) NOT NULL,
            `class` VARCHAR(32) NOT NULL,
            x FLOAT NOT NULL,
        	y FLOAT NOT NULL,
            z FLOAT NOT NULL,
        	level INT NOT NULL DEFAULT 1,
            health INT NOT NULL,
        	mana INT NOT NULL,
        	stamina INT NOT NULL DEFAULT 0,
            strength INT NOT NULL DEFAULT 0,
        	intelligence INT NOT NULL DEFAULT 0,
            experience BIGINT NOT NULL DEFAULT 0,
        	skillExperience BIGINT NOT NULL DEFAULT 0,
            gold BIGINT NOT NULL DEFAULT 0,
        	coins BIGINT NOT NULL DEFAULT 0,
            gamemaster BOOLEAN NOT NULL DEFAULT 0, 
            online INT NOT NULL,
            lastsaved DATETIME NOT NULL,
            deleted BOOLEAN NOT NULL,

        	PRIMARY KEY (name),
            INDEX(account),
        	FOREIGN KEY(account)
                REFERENCES accounts(name)
                ON DELETE CASCADE ON UPDATE CASCADE

        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;"); // convert to innodb
            //ExecuteNonQueryMySql("ALTER TABLE characters DROP PRIMARY KEY, ADD INDEX (name), ADD UNIQUE (name)");
            //ExecuteNonQueryMySql("ALTER TABLE characters ADD charcters_id INT(11) NOT NULL AUTO_INCREMENT FIRST, ADD PRIMARY KEY (charcters_id); ");
#else
        MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS characters(
            name VARCHAR(32) NOT NULL,
            account VARCHAR(32) NOT NULL,
            `class` VARCHAR(32) NOT NULL,
            x FLOAT NOT NULL,
        	y FLOAT NOT NULL,
        	level INT NOT NULL DEFAULT 1,
            health INT NOT NULL,
        	mana INT NOT NULL,
        	stamina INT NOT NULL DEFAULT 0,
            strength INT NOT NULL DEFAULT 0,
        	intelligence INT NOT NULL DEFAULT 0,
            experience BIGINT NOT NULL DEFAULT 0,
        	skillExperience BIGINT NOT NULL DEFAULT 0,
            gold BIGINT NOT NULL DEFAULT 0,
        	coins BIGINT NOT NULL DEFAULT 0,
            online INT NOT NULL,
            lastsaved DATETIME NOT NULL,
            deleted BOOLEAN NOT NULL,

        	PRIMARY KEY (name),
            INDEX(account),
        	FOREIGN KEY(account)
                REFERENCES accounts(name)
                ON DELETE CASCADE ON UPDATE CASCADE

        ) CHARACTER SET=utf8mb4");
#endif
            // -- character_inventory
#if !_iMMO2D
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_inventory(
            `character` VARCHAR(32) NOT NULL,
            slot INT NOT NULL,
        	name VARCHAR(64) NOT NULL,
            amount INT NOT NULL,
            durability INT NOT NULL,
        	summonedHealth INT NOT NULL,
            summonedLevel INT NOT NULL,
            summonedExperience BIGINT NOT NULL,
            equipmentLevel INT NOT NULL DEFAULT 0,
            equipmentGems VARCHAR(256),

            primary key(`character`, slot),
        	FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");
#else
        MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_inventory(
            `character` VARCHAR(32) NOT NULL,
            slot INT NOT NULL,
        	name VARCHAR(64) NOT NULL,
            amount INT NOT NULL,
        	summonedHealth INT NOT NULL,
            summonedLevel INT NOT NULL,
            summonedExperience BIGINT NOT NULL,
            equipmentLevel INT NOT NULL DEFAULT 0,
            equipmentGems VARCHAR(256),

            primary key(`character`, slot),
        	FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");
#endif

            // -- character_equipment
#if !_iMMO2D
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_equipment(
           `character` VARCHAR(32) NOT NULL,
            slot INT NOT NULL,
        	name VARCHAR(64) NOT NULL,
            amount INT NOT NULL,
            durability INT NOT NULL,
        	summonedHealth INT NOT NULL,
            summonedLevel INT NOT NULL,
            summonedExperience BIGINT NOT NULL,
            equipmentLevel INT NOT NULL,
            equipmentGems VARCHAR(256),

            primary key(`character`, slot),
        	FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");
#else
        MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_equipment(
           `character` VARCHAR(32) NOT NULL,
            slot INT NOT NULL,
        	name VARCHAR(64) NOT NULL,
            amount INT NOT NULL,
            summonedHealth INT NOT NULL,
            summonedLevel INT NOT NULL,
            summonedExperience BIGINT NOT NULL,
            equipmentLevel INT NOT NULL,
            equipmentGems VARCHAR(256),

            primary key(`character`, slot),
        	FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");
#endif
            // -- character_itemcooldowns
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS `character_itemcooldowns` (
            `character` varchar(32) NOT NULL,
            `category` varchar(50) NOT NULL,
            `cooldownEnd` float NOT NULL,
            UNIQUE KEY `character` (`character`,`category`),
            FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4;");

            // -- character_skills
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_skills(
            `character` VARCHAR(32) NOT NULL,
            name VARCHAR(50) NOT NULL,
            level INT NOT NULL,
        	castTimeEnd FLOAT NOT NULL,
            cooldownEnd FLOAT NOT NULL,

            PRIMARY KEY (`character`, name),
            FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");

            // -- character_buffs
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_buffs (
            `character` VARCHAR(32) NOT NULL,
            name VARCHAR(64) NOT NULL,
            level INT NOT NULL,
            buffTimeEnd FLOAT NOT NULL,

            PRIMARY KEY (`character`, name),
            FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");

#if !_iMMOQUESTS
        // -- character_quests
        MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_quests(
            `character` VARCHAR(32) NOT NULL,
            name VARCHAR(64) NOT NULL,
            progress INT NOT NULL,
        	completed BOOLEAN NOT NULL,

            PRIMARY KEY(`character`, name),
        	FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");
#endif

            // -- character_orders
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_orders(
            orderid BIGINT NOT NULL AUTO_INCREMENT,
            `character` VARCHAR(32) NOT NULL,
            coins BIGINT NOT NULL,
            processed BIGINT NOT NULL,

            PRIMARY KEY(orderid),
            INDEX(`character`),
        	FOREIGN KEY(`character`)
                REFERENCES characters(name)
                ON DELETE CASCADE ON UPDATE CASCADE
        ) CHARACTER SET=utf8mb4");

            // -- character_guild
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS character_guild(
            `character` VARCHAR(32) NOT NULL,
            guild VARCHAR(64) NOT NULL,
            `rank`INT NOT NULL,
            PRIMARY KEY(`character`)
        ) CHARACTER SET=utf8mb4");

            // -- guild_info
            MySqlHelper.ExecuteNonQuery(connectionString, @"
        CREATE TABLE IF NOT EXISTS guild_info(
            name VARCHAR(32) NOT NULL,
            notice TEXT NOT NULL,
            PRIMARY KEY(name)
        ) CHARACTER SET=utf8mb4");


            // addon system hooks
            onConnected.Invoke();
        }
#endif

    }
    // -----------------------------------------------------------------------------------
    // TryLogin
    // -----------------------------------------------------------------------------------
    public bool TryLogin(string account, string password)
    {
#if _SERVER
        if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
        {
            // demo feature: create account if it doesn't exist yet.
            MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };
            MySqlParameter passwordParameter = new("@password", MySqlDbType.VarChar) { Value = password };
            MySqlParameter createdParameter = new("@created", MySqlDbType.DateTime) { Value = DateTime.UtcNow };
            MySqlParameter lastloginParameter = new("@lastlogin", MySqlDbType.DateTime) { Value = DateTime.UtcNow };

            // Execute query
            if (allowAutoRegister)
                MySqlHelper.ExecuteNonQuery(connectionString, "INSERT IGNORE INTO accounts VALUES (@name, @password, @created, @lastlogin, 0, 0)", accountParameter, passwordParameter, createdParameter, lastloginParameter);

            // check account name, password, banned status
            bool valid = ((long)MySqlHelper.ExecuteScalar(connectionString, "SELECT Count(*) FROM accounts WHERE name=@name AND password=@password AND banned=0", accountParameter, passwordParameter)) == 1;
            if (valid)
            {
                // save last login time and return true
                MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE accounts SET lastlogin=@lastlogin WHERE name=@name", accountParameter, lastloginParameter);
                return true;
            }
        }
#endif
        return false;
    }

    public void LogoutAccountUpdate(string account, bool value)
    {
        MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };
        MySqlParameter accountOnline = new("@online", MySqlDbType.Bool) { Value = value };
        // save last login time and return true
        MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE accounts SET online = @online WHERE name=@name", accountParameter, accountOnline);
    }

    public bool IsLogged(string account)
    {
#if _SERVER
        if (!string.IsNullOrWhiteSpace(account))
        {
            MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };

            // Retourne true si Count(*) est égal à 1, sinon false
            bool t =  (long)MySqlHelper.ExecuteScalar(connectionString, "SELECT Count(*) FROM accounts WHERE name=@name AND online=1", accountParameter) == 1;
            return t;
        }
#endif
        return false;
    }


    public bool CreateAccount(string account, string password)
    {
#if _SERVER
        if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
        {
            MySqlParameter accountParameter = new("@name", MySqlDbType.VarChar) { Value = account };
            MySqlParameter passwordParameter = new("@password", MySqlDbType.VarChar) { Value = password };
            bool valid = ((long)MySqlHelper.ExecuteScalar(connectionString, "SELECT Count(*) FROM accounts WHERE name=@name AND password=@password AND banned=0", accountParameter, passwordParameter)) == 1;
            if (!valid)
            {
                MySqlParameter createdParameter = new("@created", MySqlDbType.DateTime) { Value = DateTime.UtcNow };
                MySqlParameter lastloginParameter = new("@lastlogin", MySqlDbType.DateTime) { Value = DateTime.UtcNow };
                // Execute query
                MySqlHelper.ExecuteNonQuery(connectionString, "INSERT IGNORE INTO accounts VALUES (@name, @password, @created, @lastlogin, 0)", accountParameter, passwordParameter, createdParameter, lastloginParameter);
                return true;
            }
            else
            {
                return false;
            }
        }
#endif
        return false;
    }
    // -----------------------------------------------------------------------------------
    // CharacterExists
    // -----------------------------------------------------------------------------------
    public bool CharacterExists(string characterName)
    {
#if _SERVER
        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = characterName };
        return ((long)MySqlHelper.ExecuteScalar(connectionString, "SELECT Count(*) FROM characters WHERE name=@character", characterNameParameter)) == 1;
#else
        return false;
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterDelete
    // -----------------------------------------------------------------------------------
    public void CharacterDelete(string characterName)
    {
#if _SERVER
        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = characterName };
        MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE characters SET deleted=1 WHERE name=@character", characterNameParameter);
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharactersForAccount
    // -----------------------------------------------------------------------------------
    public List<string> CharactersForAccount(string account)
    {
        List<String> result = new List<String>();
#if _SERVER
        MySqlParameter accountParameter = new("@account", MySqlDbType.VarChar) { Value = account };
        var table = MySqlHelper.ExecuteReader(connectionString,"SELECT name FROM characters WHERE account=@account AND deleted=0", accountParameter);
        foreach (var row in table)
            result.Add((string)row[0]);
#endif
        return result;
    }

    // -----------------------------------------------------------------------------------
    // LoadInventory Mysql Version
    // -----------------------------------------------------------------------------------
    private void LoadInventory(PlayerInventory inventory)
    {
#if _SERVER
        for (int i = 0; i < inventory.size; ++i)
            inventory.slots.Add(new ItemSlot());

        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = inventory.name };
#if !_iMMO2D
        using (var reader = MySqlHelper.GetReader(connectionString, @"SELECT name, slot, amount,durability, summonedHealth, summonedLevel, summonedExperience, equipmentLevel, equipmentGems FROM character_inventory WHERE `character`=@character;", characterNameParameter))
#else
        using (var reader = MySqlHelper.GetReader(connectionString, @"SELECT name, slot, amount, summonedHealth, summonedLevel, summonedExperience, equipmentLevel, equipmentGems FROM character_inventory WHERE `character`=@character;", characterNameParameter))
#endif
        {
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int slot = (int)reader["slot"];

                ScriptableItem itemData;
                if (slot < inventory.size && ScriptableItem.All.TryGetValue(itemName.GetStableHashCode(), out itemData))
                {
                    Item item = new Item(itemData);
                    int amount = (int)reader["amount"];
#if !_iMMO2D
                    item.durability = (int)reader["durability"];
#endif
                    item.summonedHealth = (int)reader["summonedHealth"];
                    item.summonedLevel = (int)reader["summonedLevel"];
                    item.summonedExperience = (long)reader["summonedExperience"];
                    item.equipmentLevel = (int)reader["equipmentLevel"];
                    item.equipmentGems = (string)reader["equipmentGems"];
                    inventory.slots[slot] = new ItemSlot(item, amount); ;
                }
            }
        }
#endif
    }

     // -----------------------------------------------------------------------------------
     // LoadEquipment
     // -----------------------------------------------------------------------------------
    private void LoadEquipment(PlayerEquipment equipment)
    {
#if _SERVER
        for (int i = 0; i < equipment.slotInfo.Length; ++i)
            equipment.slots.Add(new ItemSlot());


        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = equipment.name };
        using (var reader = MySqlHelper.GetReader(connectionString, @"SELECT * FROM character_equipment WHERE `character`=@character;", characterNameParameter))
        {
            while (reader.Read())
            {
                string itemName = (string)reader["name"];
                int slot = (int)reader["slot"];

                ScriptableItem itemData;
                if (slot < equipment.slotInfo.Length && ScriptableItem.All.TryGetValue(itemName.GetStableHashCode(), out itemData))
                {
                    Item item = new Item(itemData);
                    int amount = (int)reader["amount"];
#if !_iMMO2D
                    item.durability = (int)reader["durability"];
#endif
                    item.summonedHealth = (int)reader["summonedHealth"];
                    item.summonedLevel = (int)reader["summonedLevel"];
                    item.summonedExperience = (long)reader["summonedExperience"];
                    item.equipmentLevel = (int)reader["equipmentLevel"];
                    item.equipmentGems = (string)reader["equipmentGems"];
                    equipment.slots[slot] = new ItemSlot(item, amount);
                }
            }
        }
#endif
    }

    void LoadItemCooldowns(Player player)
    {
        // then load cooldowns
        // (one big query is A LOT faster than querying each slot separately)

        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = player.name };
        using (var reader = MySqlHelper.GetReader(connectionString, "SELECT * FROM character_itemcooldowns WHERE `character`=@character ", characterNameParameter))
        {
            while (reader.Read())
            {
                string categoryName = (string)reader["category"];
                float coldownEnd = (float)reader["cooldownEnd"];
                player.itemCooldowns.Add(categoryName, coldownEnd + NetworkTime.time);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // LoadSkills
    // -----------------------------------------------------------------------------------
    private void LoadSkills(PlayerSkills skills)
    {
#if _SERVER
        foreach (ScriptableSkill skillData in skills.skillTemplates)
            skills.skills.Add(new Skill(skillData));

        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = skills.name };
        using (var reader = MySqlHelper.GetReader(connectionString, "SELECT name, level, castTimeEnd, cooldownEnd FROM character_skills WHERE `character`=@character ", characterNameParameter))
        {
            while (reader.Read())
            {
                string skillName = (string)reader["name"];

                int index = skills.GetSkillIndexByName(skillName);
                if (index != -1)
                {
                    Skill skill = skills.skills[index];
                    skill.level = Mathf.Clamp((int)reader["level"], 1, skill.maxLevel);
                    skill.castTimeEnd = (float)reader["castTimeEnd"] + Time.time;
                    skill.cooldownEnd = (float)reader["cooldownEnd"] + Time.time;
                    skills.skills[index] = skill;
                }
#if _iMMOTRAITS
                else
                {
                    if (ScriptableSkill.All.TryGetValue(skillName.GetStableHashCode(), out ScriptableSkill skillData))
                    {
                        int level = Mathf.Clamp((int)reader["level"], 0, skillData.maxLevel);

                        Skill skill = new Skill(skillData);
                        skill.level = level;
                        skill.castTimeEnd = (float)reader["castTimeEnd"] + Time.time;
                        skill.cooldownEnd = (float)reader["cooldownEnd"] + Time.time;
                        skills.skills.Add(skill);
                    }
                }
#endif
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // LoadBuffs
    // -----------------------------------------------------------------------------------
    private void LoadBuffs(PlayerSkills skills)
    {
#if _SERVER

        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = skills.name };
        using (var reader = MySqlHelper.GetReader(connectionString, "SELECT name, level, buffTimeEnd FROM character_buffs WHERE `character` = @character ", characterNameParameter))
        {
            while (reader.Read())
            {
                string buffName = (string)reader["name"];
                if (ScriptableSkill.All.TryGetValue(buffName.GetStableHashCode(), out ScriptableSkill skillData))
                {
                    int level = Mathf.Clamp((int)reader["level"], 1, skillData.maxLevel);
                    Buff buff = new Buff((BuffSkill)skillData, level);
                    buff.buffTimeEnd = (float)reader["buffTimeEnd"] + Time.time;
                    skills.buffs.Add(buff);
                }
            }
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // LoadQuests
    // -----------------------------------------------------------------------------------
    private void LoadQuests(PlayerQuests quests)
    {
#if _SERVER

        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = quests.name };
        using var reader = MySqlHelper.GetReader(connectionString, "SELECT name, progress, completed FROM character_quests WHERE `character`=@character", characterNameParameter);
        while (reader.Read())
        {
            string questName = (string)reader["name"];
            ScriptableQuest questData;
            if (ScriptableQuest.All.TryGetValue(questName.GetStableHashCode(), out questData))
            {
                Quest quest = new Quest(questData);
                quest.progress = (int)reader["progress"];
                quest.completed = (bool)reader["completed"];
                quests.quests.Add(quest);
            }
            else Debug.LogWarning("MYSQL LoadQuests: skipped quest " + questName + " for " + quests.name + " because it doesn't exist anymore. If it wasn't removed intentionally then make sure it's in the Resources folder.");
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // LoadGuild
    // -----------------------------------------------------------------------------------
    Guild LoadGuild(string guildName)
    {
        Guild guild = new Guild();
#if _SERVER

        guild.name = guildName;

        MySqlParameter guildNameParameter = new("@guild", MySqlDbType.VarChar) { Value = guildName };

        List<List<object>> table = MySqlHelper.ExecuteReader(connectionString, "SELECT notice FROM guild_info WHERE name=@guild", guildNameParameter);
        if (table.Count == 1)
        {
            List<object> row = table[0];
            guild.notice = (string)row[0];
        }

        List<GuildMember> members = new List<GuildMember>();
        /**
        table = ExecuteReaderMySql("SELECT `character`, `rank` FROM character_guild WHERE guild=@guild", guildNameParameter);
        foreach (List<object> row in table)
        {
            GuildMember member = new GuildMember();
            member.name = (string)row[0];
            member.rank = (GuildRank)((int)row[1]);

            // is this player online right now? then use runtime data
            if (Player.onlinePlayers.TryGetValue(member.name, out Player player))
            {
                member.online = true;
                member.level = player.level.current;
            }
            else
            {
                member.online = false;
                object scalar = ExecuteScalarMySql("SELECT level FROM characters WHERE name=@character", new MySqlParameter("@character", member.name));
                member.level = scalar != null ? ((int)scalar) : 1;
            }
            members.Add(member);
        }*/

        using var reader = MySqlHelper.GetReader(connectionString, "SELECT cg.character as guild_member_name, cg.rank, c.level, c.online FROM character_guild cg LEFT JOIN characters c ON cg.character = c.name WHERE cg.guild = @guild ", guildNameParameter);
        while (reader.Read())
        {

            GuildMember member = new()
            {
                name = reader["guild_member_name"].ToString(),
                rank = (GuildRank)(int)reader["rank"],
                level = reader["level"] != null ? (int)reader["level"] : 1,
                online = (int)reader["online"] == 1
            };
            members.Add(member);
        }

        guild.members = members.ToArray();
#endif
        return guild;
    }

    // -----------------------------------------------------------------------------------
    // LoadGuildOnDemand
    // -----------------------------------------------------------------------------------
    void LoadGuildOnDemand(PlayerGuild playerGuild)
    {
#if _SERVER

        MySqlParameter characterNameParameter = new("@character", MySqlDbType.VarChar) { Value = playerGuild.name };
        string guildName = (string)MySqlHelper.ExecuteScalar(connectionString, "SELECT guild FROM character_guild WHERE `character`=@character", characterNameParameter);
        if (guildName != null)
        {
            // load guild on demand when the first player of that guild logs in
            // (= if it's not in GuildSystem.guilds yet)
            if (!GuildSystem.guilds.ContainsKey(guildName))
            {
                Guild guild = LoadGuild(guildName);
                GuildSystem.guilds[guild.name] = guild;
                playerGuild.guild = guild;
            }
            // assign from already loaded guild
            else playerGuild.guild = GuildSystem.guilds[guildName];
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad
    // -----------------------------------------------------------------------------------
    public GameObject CharacterLoad(string characterName, List<Player> prefabs, bool isPreview)
    {
#if _SERVER
        var row = MySqlHelper.ExecuteDataRow(connectionString, "SELECT * FROM characters WHERE name=@name AND deleted=0", new MySqlParameter("@name", characterName));

        if (row != null)
        {
            string className = (string)row["class"];
            var prefab = prefabs.Find(p => p.name == className);
            if (prefab != null)
            {
                GameObject go = GameObject.Instantiate(prefab.gameObject);
                Player player = go.GetComponent<Player>();

                player.name = (string)row["name"];
                player.account = (string)row["account"];
                player.className = (string)row["class"];
                float x = (float)row["x"];
                float y = (float)row["y"];
#if !_iMMO2D
                float z = (float)row["z"];
                Vector3 position = new Vector3(x, y, z);
#else
                Vector2 position = new Vector2(x, y);
#endif
                player.level.current = Mathf.Min((int)row["level"], player.level.max);
                int health = (int)row["health"];
                int mana = (int)row["mana"];
#if _iMMOSTAMINA
                int stamina = (int)row["stamina"];
#endif
                player.strength.value = (int)row["strength"];
                player.intelligence.value = (int)row["intelligence"];
                player.experience.current = (long)row["experience"];
                ((PlayerSkills)player.skills).skillExperience = (long)row["skillExperience"];
                player.gold = (long)row["gold"];
#if !_iMMO2D
                player.isGameMaster = (bool)row["gamemaster"];
#endif
                //player.isGameMaster = row.gamemaster;
                player.itemMall.coins = (long)row["coins"];

                if (player.movement.IsValidSpawnPoint(position))
                {
                    // agent.warp is recommended over transform.position and
                    // avoids all kinds of weird bugs
                    player.movement.Warp(position);
                }
                // otherwise warp to start position
                else
                {
                    Transform start = NetworkManagerMMO.GetNearestStartPosition(position);
                    player.movement.Warp(start.position);
                    // no need to show the message all the time. it would spam
                    // the server logs too much.
                    //Debug.Log(player.name + " spawn position reset because it's not on a NavMesh anymore. This can happen if the player previously logged out in an instance or if the Terrain was changed.");
                }

                LoadEquipment((PlayerEquipment)player.equipment);
                LoadInventory(player.inventory);
                LoadItemCooldowns(player);
                LoadSkills((PlayerSkills)player.skills);
                LoadBuffs((PlayerSkills)player.skills);
#if !_iMMOQUESTS
                LoadQuests(player.quests);
#endif
                LoadGuildOnDemand(player.guild);

                // addon system hooks
                onCharacterLoad.Invoke(player);
                if (!isPreview)
                    MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE characters SET online=1, lastsaved=@lastsaved WHERE name=@name", new MySqlParameter("@name", characterName), new MySqlParameter("@lastsaved", DateTime.UtcNow));

                player.health.current = health;
                player.mana.current = mana;
#if _iMMOSTAMINA
                //Debug.Log(" -> "+stamina);
                player.stamina.current = stamina;
#endif


                return go;
            }
        }
        else Debug.LogError("no prefab found for class: " + row["class"]);
#endif
                return null;
    }

    // -----------------------------------------------------------------------------------
    // SaveInventory
    // -----------------------------------------------------------------------------------
    void SaveInventory(PlayerInventory inventory, MySqlCommand command)
    {
#if _SERVER

        // Supprimer les entrées existantes pour ce personnage
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_inventory WHERE `character`=@character", new MySqlParameter("@character", inventory.name));

        // Construction de la requête INSERT groupée
#if !_iMMO2D
        string insertQuery = "INSERT INTO character_inventory (`character`, `slot`, `name`, `amount`, `durability`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#else
        string insertQuery = "INSERT INTO character_inventory (`character`, `slot`, `name`, `amount`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#endif

        List<MySqlParameter> parameters = new List<MySqlParameter>();
        for (int i = 0; i < inventory.slots.Count; ++i)
        {
            ItemSlot slot = inventory.slots[i];
            if (slot.amount > 0)
            {
                if (parameters.Count > 0)
                    insertQuery += ",";
#if !_iMMO2D
                insertQuery += "(@character_" + i + ", @slot_" + i + ", @name_" + i + ", @amount_" + i + ", @durability_" + i + ", @summonedHealth_" + i + ", @summonedLevel_" + i + ", @summonedExperience_" + i + ", @equipmentLevel_" + i + ", @equipmentGems_" + i + ")";
#else
                insertQuery += "(@character_" + i + ", @slot_" + i + ", @name_" + i + ", @amount_" + i + ", @summonedHealth_" + i + ", @summonedLevel_" + i + ", @summonedExperience_" + i + ", @equipmentLevel_" + i + ", @equipmentGems_" + i + ")";
#endif

                parameters.Add(new MySqlParameter("@character_" + i, inventory.name));
                parameters.Add(new MySqlParameter("@slot_" + i, i));
                parameters.Add(new MySqlParameter("@name_" + i, slot.item.name));
                parameters.Add(new MySqlParameter("@amount_" + i, slot.amount));
#if !_iMMO2D
                parameters.Add(new MySqlParameter("@durability_" + i, slot.item.durability));
#endif
                parameters.Add(new MySqlParameter("@summonedHealth_" + i, slot.item.summonedHealth));
                parameters.Add(new MySqlParameter("@summonedLevel_" + i, slot.item.summonedLevel));
                parameters.Add(new MySqlParameter("@summonedExperience_" + i, slot.item.summonedExperience));
                parameters.Add(new MySqlParameter("@equipmentLevel_" + i, slot.item.equipmentLevel));
                parameters.Add(new MySqlParameter("@equipmentGems_" + i, slot.item.equipmentGems));
            }
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

#endif
            }


    void SaveItemCooldowns(Player player, MySqlCommand command)
    {
        // equipment: remove old entries first, then add all new ones
        // (we could use UPDATE where slot=... but deleting everything makes
        //  sure that there are never any ghosts)
        /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_itemcooldowns WHERE `character`=@character", new MySqlParameter("@character", player.name));
        //ExecuteNonQueryMySql(command, "DELETE FROM character_itemcooldowns WHERE `character`=@character", new MySqlParameter("@character", player.name));
        foreach (KeyValuePair<string, double> kvp in player.itemCooldowns)
        {
            float cooldown = player.GetItemCooldown(kvp.Key);
            if (cooldown > 0)
            {
                //ExecuteNonQueryMySql(command, @"
                MySqlHelper.ExecuteNonQuery(connectionString, @"
                    INSERT INTO character_itemcooldowns
                    SET
                        `character` = @character,
                        category = @category,
                        cooldownEnd = @cooldownEnd",
                                    new MySqlParameter("@character", player.name),
                                    new MySqlParameter("@category", kvp.Key),
                                    new MySqlParameter("@cooldownEnd", cooldown));
            }
        }*/
        // Supprimer les entrées existantes pour ce personnage
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_itemcooldowns WHERE `character`=@character", new MySqlParameter("@character", player.name));

        // Construction de la requête INSERT groupée
        string insertQuery = "INSERT INTO character_itemcooldowns (`character`, `category`, `cooldownEnd`) VALUES ";

        List<MySqlParameter> parameters = new List<MySqlParameter>();
        int index = 0;
        foreach (KeyValuePair<string, double> kvp in player.itemCooldowns)
        {
            float cooldown = player.GetItemCooldown(kvp.Key);
            if (cooldown > 0)
            {
                if (parameters.Count > 0)
                    insertQuery += ",";

                insertQuery += "(@character_" + index + ", @category_" + index + ", @cooldownEnd_" + index + ")";

                parameters.Add(new MySqlParameter("@character_" + index, player.name));
                parameters.Add(new MySqlParameter("@category_" + index, kvp.Key));
                parameters.Add(new MySqlParameter("@cooldownEnd_" + index, cooldown));

                index++;
            }
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

    }


    // -----------------------------------------------------------------------------------
    // SaveEquipment
    // -----------------------------------------------------------------------------------
    void SaveEquipment(PlayerEquipment equipment, MySqlCommand command)
    {
#if _SERVER
        // Supprimer les entrées existantes pour ce personnage
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_equipment WHERE `character`=@character", new MySqlParameter("@character", equipment.name));

        // Construction de la requête INSERT groupée
#if !_iMMO2D
        string insertQuery = "INSERT INTO character_equipment (`character`, `slot`, `name`, `amount`, `durability`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#else
        string insertQuery = "INSERT INTO character_equipment (`character`, `slot`, `name`, `amount`, `summonedHealth`, `summonedLevel`, `summonedExperience`, `equipmentLevel`, `equipmentGems`) VALUES ";
#endif
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        int index = 0;

        for (int i = 0; i < equipment.slots.Count; ++i)
        {
            ItemSlot slot = equipment.slots[i];
            if (slot.amount > 0)
            {
                if (index > 0)
                    insertQuery += ",";
#if !_iMMO2D
                insertQuery += "(@character_" + index + ", @slot_" + index + ", @name_" + index + ", @amount_" + index + ", @durability_" + index + ", @summonedHealth_" + index + ", @summonedLevel_" + index + ", @summonedExperience_" + index + ", @equipmentLevel_" + index + ", @equipmentGems_" + index + ")";
#else
                    insertQuery += "(@character_" + index + ", @slot_" + index + ", @name_" + index + ", @amount_" + index + ", @summonedHealth_" + index + ", @summonedLevel_" + index + ", @summonedExperience_" + index + ", @equipmentLevel_" + index + ", @equipmentGems_" + index + ")";
#endif

                parameters.Add(new MySqlParameter("@character_" + index, equipment.name));
                parameters.Add(new MySqlParameter("@slot_" + index, i));
                parameters.Add(new MySqlParameter("@name_" + index, slot.item.name));
                parameters.Add(new MySqlParameter("@amount_" + index, slot.amount));
#if !_iMMO2D
                parameters.Add(new MySqlParameter("@durability_" + index, slot.item.durability));
#endif
                parameters.Add(new MySqlParameter("@summonedHealth_" + index, slot.item.summonedHealth));
                parameters.Add(new MySqlParameter("@summonedLevel_" + index, slot.item.summonedLevel));
                parameters.Add(new MySqlParameter("@summonedExperience_" + index, slot.item.summonedExperience));
                parameters.Add(new MySqlParameter("@equipmentLevel_" + index, slot.item.equipmentLevel));
                parameters.Add(new MySqlParameter("@equipmentGems_" + index, slot.item.summonedExperience));

                index++;
            }
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

#endif
            }

    // -----------------------------------------------------------------------------------
    // SaveSkills
    // -----------------------------------------------------------------------------------
    void SaveSkills(PlayerSkills skills, MySqlCommand command)
    {
#if _SERVER
        /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_skills WHERE `character`=@character", new MySqlParameter("@character", skills.name));
        //ExecuteNonQueryMySql(command, "DELETE FROM character_skills WHERE `character`=@character", new MySqlParameter("@character", skills.name));
        foreach (Skill skill in skills.skills)
        {
            if (skill.level >= 0)
            {
                //ExecuteNonQueryMySql(command, @"
                MySqlHelper.ExecuteNonQuery(connectionString, @"
                    INSERT INTO character_skills
                    SET
                        `character` = @character,
                        name = @name,
                        level = @level,
                        castTimeEnd = @castTimeEnd,
                        cooldownEnd = @cooldownEnd",
                                    new MySqlParameter("@character", skills.name),
                                    new MySqlParameter("@name", skill.name),
                                    new MySqlParameter("@level", skill.level),
                                    new MySqlParameter("@castTimeEnd", skill.CastTimeRemaining()),
                                    new MySqlParameter("@cooldownEnd", skill.CooldownRemaining()));
            }
        }*/
        // Supprimer les entrées existantes pour ce personnage
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_skills WHERE `character`=@character", new MySqlParameter("@character", skills.name));

        // Construction de la requête INSERT groupée
        string insertQuery = "INSERT INTO character_skills (`character`, `name`, `level`, `castTimeEnd`, `cooldownEnd`) VALUES ";

        List<MySqlParameter> parameters = new List<MySqlParameter>();
        int index = 0;

        foreach (Skill skill in skills.skills)
        {
            if (skill.level >= 0)
            {
                if (index > 0)
                    insertQuery += ",";

                insertQuery += "(@character_" + index + ", @name_" + index + ", @level_" + index + ", @castTimeEnd_" + index + ", @cooldownEnd_" + index + ")";

                parameters.Add(new MySqlParameter("@character_" + index, skills.name));
                parameters.Add(new MySqlParameter("@name_" + index, skill.name));
                parameters.Add(new MySqlParameter("@level_" + index, skill.level));
                parameters.Add(new MySqlParameter("@castTimeEnd_" + index, skill.CastTimeRemaining()));
                parameters.Add(new MySqlParameter("@cooldownEnd_" + index, skill.CooldownRemaining()));

                index++;
            }
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

#endif
    }

    // -----------------------------------------------------------------------------------
    // SaveBuffs
    // -----------------------------------------------------------------------------------
    void SaveBuffs(PlayerSkills skills, MySqlCommand command)
    {
#if _SERVER
        /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_buffs WHERE `character`=@character", new MySqlParameter("@character", skills.name));
        //ExecuteNonQueryMySql(command, "DELETE FROM character_buffs WHERE `character`=@character", new MySqlParameter("@character", skills.name));
        foreach (Buff buff in skills.buffs)
        {
            //ExecuteNonQueryMySql(command, "INSERT INTO character_buffs VALUES (@character, @name, @level, @buffTimeEnd)",
            MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_buffs VALUES (@character, @name, @level, @buffTimeEnd)",
                            new MySqlParameter("@character", skills.name),
                            new MySqlParameter("@name", buff.name),
                            new MySqlParameter("@level", buff.level),
                            new MySqlParameter("@buffTimeEnd", buff.BuffTimeRemaining())); //(float)buff.BuffTimeRemaining()
        }*/
        // Supprimer les entrées existantes pour ce personnage
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_buffs WHERE `character`=@character", new MySqlParameter("@character", skills.name));

        // Construction de la requête INSERT groupée
        string insertQuery = "INSERT INTO character_buffs (`character`, `name`, `level`, `buffTimeEnd`) VALUES ";

        List<MySqlParameter> parameters = new List<MySqlParameter>();
        int index = 0;

        foreach (Buff buff in skills.buffs)
        {
            if (buff.level >= 0) // Assurez-vous que le niveau est valide
            {
                if (index > 0)
                    insertQuery += ",";

                insertQuery += "(@character_" + index + ", @name_" + index + ", @level_" + index + ", @buffTimeEnd_" + index + ")";

                parameters.Add(new MySqlParameter("@character_" + index, skills.name));
                parameters.Add(new MySqlParameter("@name_" + index, buff.name));
                parameters.Add(new MySqlParameter("@level_" + index, buff.level));
                parameters.Add(new MySqlParameter("@buffTimeEnd_" + index, buff.BuffTimeRemaining()));

                index++;
            }
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

#endif
    }

    // -----------------------------------------------------------------------------------
    // SaveQuests
    // -----------------------------------------------------------------------------------
    void SaveQuests(PlayerQuests quests, MySqlCommand command)
    {
#if _SERVER
        /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_quests WHERE `character`=@character", new MySqlParameter("@character", quests.name));
        //ExecuteNonQueryMySql(command, "DELETE FROM character_quests WHERE `character`=@character", new MySqlParameter("@character", quests.name));
        foreach (Quest quest in quests.quests)
        {
            //ExecuteNonQueryMySql(command, "INSERT INTO character_quests VALUES (@character, @name, @progress, @completed)",
            MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_quests VALUES (@character, @name, @progress, @completed)",
                new MySqlParameter("@character", quests.name),
                new MySqlParameter("@name", quest.name),
                new MySqlParameter("@progress", quest.progress),
                new MySqlParameter("@completed", quest.completed));
        }*/
        // Supprimer les entrées existantes pour ce personnage
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_quests WHERE `character`=@character", new MySqlParameter("@character", quests.name));

        // Construction de la requête INSERT groupée
        string insertQuery = "INSERT INTO character_quests (`character`, `name`, `progress`, `completed`) VALUES ";

        List<MySqlParameter> parameters = new List<MySqlParameter>();
        int index = 0;

        foreach (Quest quest in quests.quests)
        {
            // Assurez-vous que les données sont valides avant de les ajouter
            if (quest.progress >= 0) // Ajustez cette condition selon les besoins
            {
                if (index > 0)
                    insertQuery += ",";

                insertQuery += "(@character_" + index + ", @name_" + index + ", @progress_" + index + ", @completed_" + index + ")";

                parameters.Add(new MySqlParameter("@character_" + index, quests.name));
                parameters.Add(new MySqlParameter("@name_" + index, quest.name));
                parameters.Add(new MySqlParameter("@progress_" + index, quest.progress));
                parameters.Add(new MySqlParameter("@completed_" + index, quest.completed));

                index++;
            }
        }

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

#endif
    }


    // -----------------------------------------------------------------------------------
    // CharacterSave
    // -----------------------------------------------------------------------------------
    void CharacterSave(Player player, bool online, MySqlCommand command)
    {
#if _SERVER
        DateTime? onlineTimestamp = null;

        if (!online)
            onlineTimestamp = DateTime.Now;
#if _iMMO2D
        var query = @"
            INSERT INTO characters
            SET
                name=@name,
                account=@account,
                class = @class,
                x = @x,
                y = @y,
                level = @level,
                health = @health,
                mana = @mana,
                stamina = @stamina,
                strength = @strength,
                intelligence = @intelligence,
                experience = @experience,
                skillExperience = @skillExperience,
                gold = @gold,
                coins = @coins,
                online = @online,
                lastsaved = @lastsaved,
                deleted = 0

            ON DUPLICATE KEY UPDATE
                account=@account,
                class = @class,
                x = @x,
                y = @y,
                level = @level,
                health = @health,
                mana = @mana,
                stamina = @stamina,
                strength = @strength,
                intelligence = @intelligence,
                experience = @experience,
                skillExperience = @skillExperience,
                gold = @gold,
                coins = @coins,
                online = @online,
                lastsaved = @lastsaved,
                deleted = 0
            ";
#else
        var query = @"
            INSERT INTO characters
            SET
                name=@name,
                account=@account,
                class = @class,
                x = @x,
                y = @y,
                z = @z,
                level = @level,
                health = @health,
                mana = @mana,
                stamina = @stamina,
                strength = @strength,
                intelligence = @intelligence,
                experience = @experience,
                skillExperience = @skillExperience,
                gold = @gold,
                coins = @coins,
                gamemaster = @gamemaster,
                online = @online,
                lastsaved = @lastsaved,
                deleted = 0

            ON DUPLICATE KEY UPDATE
                account=@account,
                class = @class,
                x = @x,
                y = @y,
                z = @z,
                level = @level,
                health = @health,
                mana = @mana,
                stamina = @stamina,
                strength = @strength,
                intelligence = @intelligence,
                experience = @experience,
                skillExperience = @skillExperience,
                gold = @gold,
                coins = @coins,
                gamemaster = @gamemaster,
                online = @online,
                lastsaved = @lastsaved,
                deleted = 0
            ";
#endif
        //ExecuteNonQueryMySql(command, query,
        MySqlHelper.ExecuteNonQuery(connectionString, query,
                    new MySqlParameter("@name", player.name),
                    new MySqlParameter("@account", player.account),
                    new MySqlParameter("@class", player.className),
                    new MySqlParameter("@x", player.transform.position.x),
                    new MySqlParameter("@y", player.transform.position.y),
#if !_iMMO2D
                    new MySqlParameter("@z", player.transform.position.z),
#endif
                    //new MySqlParameter("@ry", player.transform.rotation.x),
                    new MySqlParameter("@level", player.level.current),
                    new MySqlParameter("@health", player.health.current),
                    new MySqlParameter("@mana", player.mana.current),
                    new MySqlParameter("@stamina",
#if _iMMOSTAMINA
                    player.stamina.current
#else
                    "0"
#endif
                    ),
                    new MySqlParameter("@strength", player.strength.value),
                    new MySqlParameter("@intelligence", player.intelligence.value),
                    new MySqlParameter("@experience", player.experience.current),
                    new MySqlParameter("@skillExperience", ((PlayerSkills)player.skills).skillExperience),
                    new MySqlParameter("@gold", player.gold),
                    new MySqlParameter("@coins", player.itemMall.coins),
#if !_iMMO2D
                    new MySqlParameter("@gamemaster", player.isGameMaster),
#endif
                    new MySqlParameter("@online", online ? 1 : 0),
                    new MySqlParameter("@lastsaved", DateTime.UtcNow)
                );

#if _iMMOLOBBY
        if(!online)
        {
            LogoutAccountUpdate(player.account, online);
        }
#endif
        SaveInventory(player.inventory, command);
        SaveEquipment((PlayerEquipment)player.equipment, command);
        SaveItemCooldowns(player, command);
        SaveSkills((PlayerSkills)player.skills, command);
        SaveBuffs((PlayerSkills)player.skills, command);
#if !_iMMOQUESTS
        SaveQuests(player.quests, command);
#endif
        if (player.guild.InGuild())
            SaveGuild(player.guild.guild, false); // TODO only if needs saving? but would be complicated

        // addon system hooks
        onCharacterSave.Invoke(player);

#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave
    // -----------------------------------------------------------------------------------
    public void CharacterSave(Player player, bool online, bool useTransaction = true)
    {
#if _SERVER
        MySqlHelper.ExecuteTransaction(connectionString, command =>
        {
            CharacterSave(player, online, command);
        });
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSaveMany
    // -----------------------------------------------------------------------------------

    public void CharacterSaveMany(IEnumerable<Player> players, bool online = true)
    {
#if _SERVER && !_iMMOTHREADDB
        try
        {
            MySqlHelper.ExecuteTransaction(connectionString, command =>
            {
                foreach (Player player in players)
                {
                    CharacterSave(player, online, command);
                }
            });
            /*Transaction(command =>
            {
                foreach (Player player in players)
                {
                    CharacterSave(player, online, command);
                }
            });*/
        }
        catch(Exception e)
        {
            Debug.Log("SQL error :" + e );
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // SaveGuild
    // -----------------------------------------------------------------------------------
    public void SaveGuild(Guild guild, bool useTransaction = true)
    {
#if _SERVER
        MySqlHelper.ExecuteTransaction(connectionString, command =>
        {
            var query = @"INSERT INTO guild_info SET `name` = @guild, `notice` = @notice ON DUPLICATE KEY UPDATE `notice` = @notice";
            MySqlHelper.ExecuteNonQuery(connectionString, query,
                                new MySqlParameter("@guild", guild.name),
                                new MySqlParameter("@notice", guild.notice));

            /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_guild WHERE `guild` = @guild", new MySqlParameter("@guild", guild.name));

            var query2 = @"INSERT INTO character_guild SET `guild` = @guild, `rank`= @rank, `character`= @character
                            ON DUPLICATE KEY UPDATE `guild` = @guild, `rank`= @rank, `character`= @character";

            foreach (GuildMember member in guild.members)
            {
                // ExecuteNonQueryMySql(command, query2,
                MySqlHelper.ExecuteNonQuery(connectionString, query2,
                                new MySqlParameter("@guild", guild.name),
                                new MySqlParameter("@rank", member.rank),
                                new MySqlParameter("@character", member.name));
            }*/

            // Supprimer les entrées existantes pour cette guilde
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_guild WHERE `guild` = @guild", new MySqlParameter("@guild", guild.name));

            // Construction de la requête INSERT groupée avec ON DUPLICATE KEY UPDATE
            string insertQuery = "INSERT INTO character_guild (`guild`, `rank`, `character`) VALUES ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            int index = 0;

            foreach (GuildMember member in guild.members)
            {
                if (parameters.Count > 0)
                    insertQuery += ",";

                insertQuery += "(@guild_" + index + ", @rank_" + index + ", @character_" + index + ")";

                parameters.Add(new MySqlParameter("@guild_" + index, guild.name));
                parameters.Add(new MySqlParameter("@rank_" + index, member.rank));
                parameters.Add(new MySqlParameter("@character_" + index, member.name));

                index++;
            }

            // Ajout de la clause ON DUPLICATE KEY UPDATE
            insertQuery += " ON DUPLICATE KEY UPDATE `rank` = VALUES(`rank`), `character` = VALUES(`character`)";

            // Exécution de la requête si des paramètres ont été ajoutés
            if (parameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
            }

        });
        /*Transaction(command =>
        {
            var query = @"INSERT INTO guild_info SET `name` = @guild, `notice` = @notice ON DUPLICATE KEY UPDATE `notice` = @notice";
            MySqlHelper.ExecuteNonQuery(connectionString, query,
                                new MySqlParameter("@guild", guild.name),
                                new MySqlParameter("@notice", guild.notice));

            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_guild WHERE `guild` = @guild", new MySqlParameter("@guild", guild.name));

            var query2 = @"INSERT INTO character_guild SET `guild` = @guild, `rank`= @rank, `character`= @character
                            ON DUPLICATE KEY UPDATE `guild` = @guild, `rank`= @rank, `character`= @character";

            foreach (GuildMember member in guild.members)
            {
                // ExecuteNonQueryMySql(command, query2,
                MySqlHelper.ExecuteNonQuery(connectionString, query2,
                                new MySqlParameter("@guild", guild.name),
                                new MySqlParameter("@rank", member.rank),
                                new MySqlParameter("@character", member.name));
            }
        });*/
#endif
    }

    // -----------------------------------------------------------------------------------
    // GuildExists
    // -----------------------------------------------------------------------------------
    public bool GuildExists(string guild)
    {
#if _SERVER
        return ((long)MySqlHelper.ExecuteScalar(connectionString, "SELECT Count(*) FROM guild_info WHERE `name`=@name", new MySqlParameter("@name", guild))) == 1;
#else
        return false;
#endif
    }

    // -----------------------------------------------------------------------------------
    // RemoveGuild
    // -----------------------------------------------------------------------------------
    public void RemoveGuild(string guild)
    {
#if _SERVER
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM guild_info WHERE `name`=@name", new MySqlParameter("@name", guild));
        MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_guild WHERE guild=@guild", new MySqlParameter("@guild", guild));
#endif
    }

    // -----------------------------------------------------------------------------------
    // GrabCharacterOrders
    // -----------------------------------------------------------------------------------
    public List<long> GrabCharacterOrders(string characterName)
    {
        var result = new List<long>();
#if _SERVER
        var table = MySqlHelper.ExecuteReader(connectionString, "SELECT orderid, coins FROM character_orders WHERE `character`=@character AND processed=0", new MySqlParameter("@character", characterName));

        /*foreach (var row in table)
        {
            result.Add((long)row[1]);
            MySqlHelper.ExecuteNonQuery(connectionString, "UPDATE character_orders SET processed=1 WHERE orderid=@orderid", new MySqlParameter("@orderid", (long)row[0]));
        }*/

        // Liste pour stocker les paramètres
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        string updateQuery = "UPDATE character_orders SET processed=1 WHERE orderid IN (";

        // Construction de la requête UPDATE groupée
        for (int i = 0; i < table.Count; ++i)
        {
            if (i > 0)
                updateQuery += ",";

            updateQuery += "@orderid_" + i;
            parameters.Add(new MySqlParameter("@orderid_" + i, (long)table[i][0]));

            // Ajouter l'ordre dans la liste des résultats
            result.Add((long)table[i][1]);
        }

        updateQuery += ")";

        // Exécution de la requête si des paramètres ont été ajoutés
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, updateQuery, parameters.ToArray());
        }

#endif
        return result;
    }

    // -----------------------------------------------------------------------------------
}
#endif
#if _SERVER && _iMMOCRAFTING

#if _MYSQL
using MySqlConnector;
using System.Collections.Generic;
#endif

// DATABASE (SQLite / mySQL Hybrid)
public partial class Database
{

#if _SQLITE
	// -----------------------------------------------------------------------------------
    // Character Crafts
    // -----------------------------------------------------------------------------------
    class character_crafts
    {
        public string character { get; set; }
        public string profession { get; set; }
        public long experience { get; set; }
    }

    // -----------------------------------------------------------------------------------
    // Character Recipes
    // -----------------------------------------------------------------------------------
    class character_recipes
    {
        public string character { get; set; }
        public string recipe { get; set; }
    }
#endif
    private void Start_Tools_Crafting()
    {
        onConnected.AddListener(Connect_Crafting);
        onCharacterLoad.AddListener(CharacterLoad_Crafting);
        onCharacterSave.AddListener(CharacterSave_Crafting);
    }
    // -----------------------------------------------------------------------------------
    // Connect_Crafting
    // -----------------------------------------------------------------------------------
    private void Connect_Crafting()
    {
#if _MYSQL
        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS character_crafts (
			`character` VARCHAR(32) NOT NULL,
			profession VARCHAR(32) NOT NULL,
			experience BIGINT
            ) CHARACTER SET=utf8mb4");

        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS character_recipes (
			`character` VARCHAR(32) NOT NULL,
			recipe VARCHAR(32) NOT NULL
             ) CHARACTER SET=utf8mb4");
#elif _SQLITE
        connection.CreateTable<character_crafts>();
        connection.CreateIndex(nameof(character_crafts), new[] { "character", "profession" });
        connection.CreateTable<character_recipes>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_Crafting
    // -----------------------------------------------------------------------------------
    private void CharacterLoad_Crafting(Player player)
    {
        if (player.playerCraftingExtended)
        {
#if _MYSQL
            var table = MySqlHelper.ExecuteReader(connectionString, "SELECT profession, experience FROM character_crafts WHERE `character`=@character",
                        new MySqlParameter("@character", player.name));

            foreach (var row in table)
            {
                CraftingProfession profession = new CraftingProfession((string)row[0]);
                profession.experience = (long)row[1];
                player.playerCraftingExtended.Crafts.Add(profession);
            }

            var table2 = MySqlHelper.ExecuteReader(connectionString, "SELECT recipe FROM character_recipes WHERE `character`=@name", new MySqlParameter("@name", player.name));
            foreach (var row in table2)
            {
                player.playerCraftingExtended._recipes.Add((string)row[0]);
            }
#elif _SQLITE
        var table = connection.Query<character_crafts>("SELECT profession, experience FROM character_crafts WHERE character=?", player.name);
        foreach (var row in table)
        {
            CraftingProfession profession = new CraftingProfession(row.profession);
            profession.experience = row.experience;
            player.playerCraftingExtended.Crafts.Add(profession);
        }

        var table2 = connection.Query<character_recipes>("SELECT recipe FROM character_recipes WHERE character=?", player.name);
        foreach (var row in table2)
            player.playerCraftingExtended._recipes.Add(row.recipe);
#endif
        }
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_Crafting
    // -----------------------------------------------------------------------------------
    private void CharacterSave_Crafting(Player player)
    {
        if (player.playerCraftingExtended)
        {
#if _MYSQL
            /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_crafts WHERE `character`=@character", new MySqlParameter("@character", player.name));
            foreach (var profession in player.playerCraftingExtended.Crafts)
                MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_crafts VALUES (@character, @profession, @experience)",
                                new MySqlParameter("@character", player.name),
                                new MySqlParameter("@profession", profession.templateName),
                                new MySqlParameter("@experience", profession.experience));

            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_recipes WHERE `character`=@character", new MySqlParameter("@character", player.name));
            for (int i = 0; i < player.playerCraftingExtended._recipes.Count; ++i)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_recipes VALUES (@character, @recipe)",
                     new MySqlParameter("@character", player.name),
                     new MySqlParameter("@recipe", player.playerCraftingExtended._recipes[i]));
            }*/

            // Suppression des anciennes entrées pour le personnage dans character_crafts
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_crafts WHERE `character`=@character", new MySqlParameter("@character", player.name));

            // Construction de la requête INSERT groupée pour character_crafts
            string insertCraftsQuery = "INSERT INTO character_crafts (`character`, `profession`, `experience`) VALUES ";
            List<MySqlParameter> craftParameters = new List<MySqlParameter>();

            for (int i = 0; i < player.playerCraftingExtended.Crafts.Count; i++)
            {
                if (i > 0)
                    insertCraftsQuery += ",";

                insertCraftsQuery += "(@character_" + i + ", @profession_" + i + ", @experience_" + i + ")";
                craftParameters.Add(new MySqlParameter("@character_" + i, player.name));
                craftParameters.Add(new MySqlParameter("@profession_" + i, player.playerCraftingExtended.Crafts[i].templateName));
                craftParameters.Add(new MySqlParameter("@experience_" + i, player.playerCraftingExtended.Crafts[i].experience));
            }

            // Exécution de la requête si des paramètres ont été ajoutés
            if (craftParameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertCraftsQuery, craftParameters.ToArray());
            }

            // Suppression des anciennes entrées pour le personnage dans character_recipes
            MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_recipes WHERE `character`=@character", new MySqlParameter("@character", player.name));

            // Construction de la requête INSERT groupée pour character_recipes
            string insertRecipesQuery = "INSERT INTO character_recipes (`character`, `recipe`) VALUES ";
            List<MySqlParameter> recipeParameters = new List<MySqlParameter>();

            for (int i = 0; i < player.playerCraftingExtended._recipes.Count; i++)
            {
                if (i > 0)
                    insertRecipesQuery += ",";

                insertRecipesQuery += "(@character_" + i + ", @recipe_" + i + ")";
                recipeParameters.Add(new MySqlParameter("@character_" + i, player.name));
                recipeParameters.Add(new MySqlParameter("@recipe_" + i, player.playerCraftingExtended._recipes[i]));
            }

            // Exécution de la requête si des paramètres ont été ajoutés
            if (recipeParameters.Count > 0)
            {
                MySqlHelper.ExecuteNonQuery(connectionString, insertRecipesQuery, recipeParameters.ToArray());
            }

#elif _SQLITE
        connection.Execute("DELETE FROM character_crafts WHERE character=?", player.name);
        foreach (var profession in player.playerCraftingExtended.Crafts)
            connection.InsertOrReplace(new character_crafts
            {
                character = player.name,
                profession = profession.templateName,
                experience = profession.experience
            });

        connection.Execute("DELETE FROM character_recipes WHERE character=?", player.name);
        for (int i = 0; i < player.playerCraftingExtended._recipes.Count; ++i)
        {
            connection.InsertOrReplace(new character_recipes
            {
                character = player.name,
                recipe = player.playerCraftingExtended._recipes[i]
            });
        }
#endif
        }
    }
    // -----------------------------------------------------------------------------------
}

#endif
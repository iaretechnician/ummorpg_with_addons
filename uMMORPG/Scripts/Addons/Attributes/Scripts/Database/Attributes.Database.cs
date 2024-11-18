#if  _SERVER && _iMMOATTRIBUTES

using UnityEngine;
using System.Collections.Generic;

#if _MYSQL
using MySqlConnector;
#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{

#if _SQLITE
    // -----------------------------------------------------------------------------------
    // character_attributes
    // -----------------------------------------------------------------------------------
    class character_attributes
    {
        public string character { get; set; }
        public int slot { get; set; }
        public string name { get; set; }
        public int points { get; set; }
    }
#endif


    private void Start_Tools_Attributes()
    {
        onConnected.AddListener(Connect_Attributes);
        onCharacterLoad.AddListener(CharacterLoad_Attributes);
        onCharacterSave.AddListener(CharacterSave_Attributes);
    }

    // -----------------------------------------------------------------------------------
    // Connec_Attributes
    // -----------------------------------------------------------------------------------
    private void Connect_Attributes()
    {
#if _MYSQL && _iMMOATTRIBUTES
        MySqlHelper.ExecuteNonQuery(connectionString, @"CREATE TABLE IF NOT EXISTS character_attributes (
                        `character` VARCHAR(32) NOT NULL,
                        slot INTEGER NOT NULL,
                        name TEXT NOT NULL,
                        points INTEGER NOT NULL
                        ) CHARACTER SET=utf8mb4");
#elif _SQLITE && _iMMOATTRIBUTES
        connection.CreateTable<character_attributes>();
#endif
    }


    public void LoadAttributes(Player player)
    {
#if _MYSQL && _iMMOATTRIBUTES
        foreach (AttributeTemplate template in player.playerAttribute.playerAttributes.AttributeTypes)
        {
            if (template == null) continue;
            Attribute attr = new Attribute(template);
            var table = MySqlHelper.ExecuteReader(connectionString, "SELECT points FROM character_attributes WHERE `character`=@character AND name=@name", new MySqlParameter("@character", player.name), new MySqlParameter("@name", attr.name));
            if (table.Count == 1)
            {
                var row = table[0];
                attr.points = (int)row[0];
            }
            //Debug.Log("<>" + template.name + " Point :" + attr.points);
            player.playerAttribute.Attributes.Add(attr);
            int total = 0;
            if (player.playerAttribute.Attributes.Count > 0)
            {
                int points = 0;
                int flatBonus = 0;
                float pctBonus = 0f;

                foreach (Attribute attrib in player.playerAttribute.Attributes)
                {
                    points = player.playerAttribute.calculateBonusAttribute(attrib) + attrib.points;

                    flatBonus += attrib.flatHealth * points;
                    pctBonus += attrib.percentHealth * points;
                }

                total += flatBonus;
                total += (int)Mathf.Round(player.health.baseHealth.Get(player.level.current) * pctBonus);
            }
            player.health.current = total;
        }
#elif _SQLITE && _iMMOATTRIBUTES
        foreach (AttributeTemplate template in player.playerAttribute.playerAttributes.AttributeTypes)
        {
            if (template == null) continue;
            Attribute attr = new Attribute(template);
            var table = connection.Query<character_attributes>("SELECT points FROM character_attributes WHERE character=? AND name=?", player.name, attr.name);
            if (table.Count == 1)
            {
                var row = table[0];
                attr.points = row.points;
            }
            player.playerAttribute.Attributes.Add(attr);
            int total = 0;
            if (player.playerAttribute.Attributes.Count > 0)
            {
                int points = 0;
                int flatBonus = 0;
                float pctBonus = 0f;

                foreach (Attribute attrib in player.playerAttribute.Attributes)
                {
                    points = player.playerAttribute.calculateBonusAttribute(attrib) + attrib.points;

                    flatBonus += attrib.flatHealth * points;
                    pctBonus += attrib.percentHealth * points;
                }

                total += flatBonus;
                total += (int)Mathf.Round(player.health.baseHealth.Get(player.level.current) * pctBonus);
            }
            player.health.current = total;
        }
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_Attributes
    // -----------------------------------------------------------------------------------
    private void CharacterLoad_Attributes(Player player)
    {
#if _MYSQL && _iMMOATTRIBUTES
        /*foreach (AttributeTemplate template in player.playerAttribute.playerAttributes.AttributeTypes) {
            if (template == null) continue;
            Attribute attr = new Attribute(template);
            var table = MySqlHelper.ExecuteReader(connectionString, "SELECT points FROM character_attributes WHERE `character`=@character AND name=@name", new MySqlParameter("@character", player.name), new MySqlParameter("@name", attr.name));
            if (table.Count == 1) {
                var row = table[0];
                attr.points = (int)row[0];
            }
            player.playerAttribute.Attributes.Add(attr);
            int total = 0;
            if (player.playerAttribute.Attributes.Count > 0)
            {
                int points = 0;
                int flatBonus = 0;
                float pctBonus = 0f;

                foreach (Attribute attrib in player.playerAttribute.Attributes)
                {
                    points = player.playerAttribute.calculateBonusAttribute(attrib) + attrib.points;

                    flatBonus += attrib.flatHealth * points;
                    pctBonus += attrib.percentHealth * points;
                }

                total += flatBonus;
                total += (int)Mathf.Round(player.health.baseHealth.Get(player.level.current) * pctBonus);
            }
           
        }*/

        // Récupération de tous les attributs pour ce personnage en une seule requête
        var table = MySqlHelper.ExecuteReader(connectionString, "SELECT name, points FROM character_attributes WHERE `character`=@character", new MySqlParameter("@character", player.name));

        // Création d'un dictionnaire pour un accès rapide aux attributs récupérés
        Dictionary<string, int> attributesDict = new Dictionary<string, int>();
        foreach (var row in table)
        {
            attributesDict[(string)row[0]] = (int)row[1];
        }

        // Boucle sur les templates d'attributs
        foreach (AttributeTemplate template in player.playerAttribute.playerAttributes.AttributeTypes)
        {
            if (template == null) continue;

            Attribute attr = new Attribute(template);

            // Si l'attribut existe dans la base de données, on récupère les points associés
            if (attributesDict.TryGetValue(attr.name, out int points))
            {
                attr.points = points;
            }

            player.playerAttribute.Attributes.Add(attr);
        }

        // Calcul des bonus en fonction des attributs du joueur
        int total = 0;
        if (player.playerAttribute.Attributes.Count > 0)
        {
            int flatBonus = 0;
            float pctBonus = 0f;

            foreach (Attribute attrib in player.playerAttribute.Attributes)
            {
                int points = player.playerAttribute.calculateBonusAttribute(attrib) + attrib.points;

                flatBonus += attrib.flatHealth * points;
                pctBonus += attrib.percentHealth * points;
            }

            total += flatBonus;
            total += (int)Mathf.Round(player.health.baseHealth.Get(player.level.current) * pctBonus);
        }

#elif _SQLITE && _iMMOATTRIBUTES
        foreach (AttributeTemplate template in player.playerAttribute.playerAttributes.AttributeTypes)
        {
            if (template == null) continue;
            Attribute attr = new Attribute(template);
            var table = connection.Query<character_attributes>("SELECT points FROM character_attributes WHERE character=? AND name=?", player.name, attr.name);
            if (table.Count == 1)
            {
                var row = table[0];
                attr.points = row.points;
            }
            player.playerAttribute.Attributes.Add(attr);
            int total = 0;
            if (player.playerAttribute.Attributes.Count > 0)
            {
                int points = 0;
                int flatBonus = 0;
                float pctBonus = 0f;

                foreach (Attribute attrib in player.playerAttribute.Attributes)
                {
                    points = player.playerAttribute.calculateBonusAttribute(attrib) + attrib.points;

                    flatBonus += attrib.flatHealth * points;
                    pctBonus += attrib.percentHealth * points;
                }

                total += flatBonus;
                total += (int)Mathf.Round(player.health.baseHealth.Get(player.level.current) * pctBonus);
            }
           
        }

#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_Attributes
    // -----------------------------------------------------------------------------------
    private void CharacterSave_Attributes(Player player)
    {
#if _MYSQL && _iMMOATTRIBUTES
        /*MySqlHelper.ExecuteNonQuery(connectionString, "DELETE FROM character_attributes WHERE `character`=@character", new MySqlParameter("@character", player.name));
        for (int i = 0; i < player.playerAttribute.Attributes.Count; ++i) {
            var attr = player.playerAttribute.Attributes[i];
            MySqlHelper.ExecuteNonQuery(connectionString, "INSERT INTO character_attributes VALUES (@character, @slot, @name, @points)",
                            new MySqlParameter("@character", player.name),
                            new MySqlParameter("@slot", i),
                            new MySqlParameter("@name", attr.name),
                            new MySqlParameter("@points", attr.points));
        }*/

        // Supprimer les anciens attributs du personnage
        MySqlHelper.ExecuteNonQuery(connectionString,
            "DELETE FROM character_attributes WHERE `character`=@character",
            new MySqlParameter("@character", player.name));

        // Préparation de la requête d'insertion groupée
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        string insertQuery = "INSERT INTO character_attributes (`character`, `slot`, `name`, `points`) VALUES ";

        // Boucle pour construire la requête d'insertion groupée
        for (int i = 0; i < player.playerAttribute.Attributes.Count; ++i)
        {
            var attr = player.playerAttribute.Attributes[i];

            if (i > 0)
                insertQuery += ",";

            insertQuery += "(@character_" + i + ", @slot_" + i + ", @name_" + i + ", @points_" + i + ")";

            parameters.Add(new MySqlParameter("@character_" + i, player.name));
            parameters.Add(new MySqlParameter("@slot_" + i, i));
            parameters.Add(new MySqlParameter("@name_" + i, attr.name));
            parameters.Add(new MySqlParameter("@points_" + i, attr.points));
        }

        // Exécuter la requête groupée si des données sont présentes
        if (parameters.Count > 0)
        {
            MySqlHelper.ExecuteNonQuery(connectionString, insertQuery, parameters.ToArray());
        }

#elif _SQLITE && _iMMOATTRIBUTES
        connection.Execute("DELETE FROM character_attributes WHERE character=?", player.name);
        for (int i = 0; i < player.playerAttribute.Attributes.Count; ++i)
        {
            var attr = player.playerAttribute.Attributes[i];
            connection.InsertOrReplace(new character_attributes
            {
                character = player.name,
                slot = i,
                name = attr.name,
                points = attr.points
            });
        }
#endif
    }

    // -----------------------------------------------------------------------------------
}
#endif
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// WORLD EVENT

[CreateAssetMenu(fileName = "New WorldEvent", menuName = "ADDON/Templates/New WorldEvent", order = 999)]
public partial class WorldEventTemplate : ScriptableObject
{
    [Header("[EVENT THRESHOLDS (checked top to bottom)]")]
    public WorldEventData[] thresholdData;

    // -----------------------------------------------------------------------------------
    // Caching
    // -----------------------------------------------------------------------------------
    static Dictionary<int, WorldEventTemplate> cache;
    public static Dictionary<int, WorldEventTemplate> All
    {
        get
        {
            // not loaded yet?
            if (cache == null)
            {
                WorldEventTemplate[] entry = Resources.LoadAll<WorldEventTemplate>("");
             
                // check for duplicates, then add to cache
                List<string> duplicates = entry.ToList().FindDuplicates(item => item.name);
                if (duplicates.Count == 0)
                {
                    cache = entry.ToDictionary(item => item.name.GetStableHashCode(), item => item);
                }
                else
                {
                    foreach (string duplicate in duplicates)
                        Debug.LogError("Resources folder contains multiple WorldEventTemplate with the name " + duplicate + ". If you are using subfolders like 'WorldEvent/EventName' and 'Resources/EventName' and rename it.");
                }
            }
            return cache;
        }
    }
    // -----------------------------------------------------------------------------------
}

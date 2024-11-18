using UnityEngine;

// DATABASE CLEANER
[CreateAssetMenu(fileName = "DatabaseCleaner", menuName = "ADDON/Templates/New DatabaseCleaner", order = 999)]
public class Tmpl_DatabaseCleaner : ScriptableObject
{
    [Tooltip("One click deactivation")]
    public bool isActive = true;

    [Tooltip("Delete inactive accounts after x days on server start (set 0 to disable)")]
    public int PruneInactiveAfterDays = 1;

    [Tooltip("Delete banned accounts after x days on server start (set 0 to disable)")]
    public int PruneBannedAfterDays = 1;

    [Tooltip("Delete empty accounts (= 0 chars) after x days on server start (set 0 to disable)")]
    public bool PruneEmptyAccounts = true;
    [Tooltip("Delete banned accounts after x days on server start (set 0 to disable)")]
    public int PruneeEmptyAccountsAfterDays = 1;
    public string[] characterTables;
    public string[] accountTables;
}
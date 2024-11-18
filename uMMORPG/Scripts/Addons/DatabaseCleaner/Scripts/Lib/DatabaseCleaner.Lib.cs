#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_DatabaseCleaner()
    {
        AddOn addon = new AddOn();

        addon.name          = "DatabaseCleaner";
        addon.basis         = "uMMORPG3d Remastered";
        addon.define        = "_iMMODBCLEANER";
        addon.author        = "Trugord";
        addon.version       = "2020.3.37-37.1";
        addon.dependencies  = "none";
        addon.comments      = "none";
        addon.active        = true;

        addons.Add(addon);
    }
}
#endif
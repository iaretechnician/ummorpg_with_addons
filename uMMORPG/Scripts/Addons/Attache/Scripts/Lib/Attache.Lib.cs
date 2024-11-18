#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_Attache()
    {
        AddOn addon = new()
        {
            name = "Attache",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOATTACHE",
            author = "Trugord",
            version = "",
            dependencies = "Tools",
            comments = "none",
            active = true
        };
        addons.Add(addon);
    }
}
#endif
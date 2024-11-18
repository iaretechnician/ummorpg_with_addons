#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_Emotes()
    {
        AddOn addon = new()
        {
            name = "Emotes",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOEMOTES",
            author = "Trugord",
            version = "",
            dependencies = "Tools, CompleteChat",
            comments = "none",
            active = true
        };
        addons.Add(addon);
    }
}
#endif
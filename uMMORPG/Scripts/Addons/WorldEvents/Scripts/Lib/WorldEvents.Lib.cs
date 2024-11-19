#if UNITY_EDITOR && _iMMOTOOLS
public partial class DefinesManager
{
    public static void Constructor_WorldEvents()
    {
        AddOn addon = new()
        {
            name = "World Events",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOWORLDEVENTS",
            author = "Trugord",
            version = "",
            dependencies = "_Tools, Extended Quests",
            comments = "none",
            active = true
        };

        addons.Add(addon);
    }
}
#endif
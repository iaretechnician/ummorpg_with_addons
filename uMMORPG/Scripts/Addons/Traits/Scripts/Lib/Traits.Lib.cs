#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_Traits()
    {
        AddOn addon = new()
        {
            name = "Traits",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOTRAITS",
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
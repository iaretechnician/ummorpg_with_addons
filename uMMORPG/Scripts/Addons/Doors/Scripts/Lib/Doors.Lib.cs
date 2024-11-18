#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_Doors()
    {
        AddOn addon = new()
        {
            name = "Doors",
            basis = "uMMORPG3d Remastered",
            define = "_iMMODOORS",
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
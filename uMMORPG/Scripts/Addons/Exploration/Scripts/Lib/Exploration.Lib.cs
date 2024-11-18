#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_Exploration()
    {
        AddOn addon = new()
        {
            name = "Exploration",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOEXPLORATION",
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
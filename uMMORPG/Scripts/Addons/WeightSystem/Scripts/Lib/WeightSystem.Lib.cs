#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_WeightSystem()
    {
        AddOn addon = new()
        {
            name = "Weight System",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOWEIGHTSYSTEM",
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
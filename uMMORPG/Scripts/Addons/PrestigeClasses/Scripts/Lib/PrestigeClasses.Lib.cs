#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_PrestigeClasses()
    {
        AddOn addon = new()
        {
            name = "PrestigeClasses",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOPRESTIGECLASSES",
            author = "Trugord",
            version = "",
            dependencies = "_Tools,UsageRequirements,Traits",
            comments = "Install All Other Addon before this",
            active = true
        };

        addons.Add(addon);
    }
}
#endif
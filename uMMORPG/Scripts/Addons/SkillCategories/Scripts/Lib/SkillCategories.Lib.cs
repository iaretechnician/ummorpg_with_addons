#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_SkillCategories()
    {
        AddOn addon = new()
        {
            name = "SkillCategories",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOSKILLCATEGORY",
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
#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_AnimationTags()
    {
        AddOn addon = new()
        {
            name = "Animation Tags",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOANIMATIONTAGS",
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
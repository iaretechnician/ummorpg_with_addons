#if UNITY_EDITOR
public partial class DefinesManager
{
    public static void Constructor_CursedEquipment()
    {
        AddOn addon = new()
        {
            name = "CursedEquipment",
            basis = "uMMORPG3d Remastered",
            define = "_iMMOCURSEDEQUIPMENT",
            author = "Trugord",
            version = "",
            dependencies = "none",
            comments = "none",
            active = true
        };

        addons.Add(addon);
    }
}
#endif
using System.ComponentModel;

namespace ReactIb.Enums
{
    public enum FADataType
    {
        [Description("")] Undefined = 0,
        [Description("GROUPS")] Groups = 1,
        [Description("PROFILES")] Profiles = 2,
        [Description("ALIASES")] Aliases = 3
    }
}
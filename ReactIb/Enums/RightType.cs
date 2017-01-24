using System.ComponentModel;

namespace ReactIb.Enums
{
    public enum RightType
    {
        [Description("P")] Put,
        [Description("C")] Call,
        [Description("")] Undefined
    }
}
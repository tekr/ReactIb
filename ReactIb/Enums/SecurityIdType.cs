using System.ComponentModel;

namespace ReactIb.Enums
{
    public enum SecurityIdType
    {
        [Description("")] None,
        [Description("ISIN")] Isin,
        [Description("CUSIP")] Cusip,
        [Description("SEDOL")] Sedol,
        [Description("RIC")] Ric
    }
}

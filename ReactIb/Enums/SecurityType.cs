using System.ComponentModel;

namespace ReactIb.Enums
{
    public enum SecurityType
    {
        [Description("STK")] Stock,
        [Description("OPT")] Option,
        [Description("FUT")] Future,
        [Description("IND")] Index,
        [Description("FOP")] FutureOption,
        [Description("CASH")] Cash,
        /// <summary>
        /// For Combination Orders - must use combo leg details
        /// </summary>
        [Description("BAG")] Bag,
        [Description("BOND")] Bond,
        [Description("WAR")] Warrant,
        [Description("CMDTY")] Commodity,
        [Description("BILL")] Bill,
        [Description("")] Undefined
    }
}
using System;
using System.ComponentModel;

namespace ReactIb.Enums
{
    [Serializable] 
    public enum BarSize
    {
        [Description("1 secs")] OneSecond = 1,
        [Description("5 secs")] FiveSeconds = 2,
        [Description("15 secs")] FifteenSeconds = 3,
        [Description("30 secs")] ThirtySeconds = 4,
        [Description("1 min")] OneMinute = 5,
        [Description("2 mins")] TwoMinutes = 6,
        [Description("5 mins")] FiveMinutes = 7,
        [Description("15 mins")] FifteenMinutes = 8,
        [Description("30 mins")] ThirtyMinutes = 9,
        [Description("1 hour")] OneHour = 10,
        [Description("1 day")] OneDay = 11,
        [Description("1 week")] OneWeek = 12,
        [Description("1 month")] OneMonth = 13,

        // Doesn't seem to work when tested
        //[Description("1 year")] OneYear = 14
    }
}

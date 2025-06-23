using System.ComponentModel;

namespace ServicePortals.Domain.Enums
{
    public enum TimeLeaveEnum
    {
        [Description("Nghỉ cả ngày")]
        AlldayLeave = 1,

        [Description("Nghỉ sáng")]
        MorningLeave = 2,

        [Description("Nghỉ chiều")]
        AfternoonLeave = 3
    }
}

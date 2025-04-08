using System.ComponentModel;

namespace ServicePortal.Domain.Enums
{
    public enum TypeLeave
    {
        [Description("Nghỉ cả ngày")]
        AlldayLeave = 1,

        [Description("Nghỉ sáng")]
        MorningLeave = 2,

        [Description("Nghỉ chiều")]
        AfternoonLeave = 3
    }
}

using System.ComponentModel;

namespace ServicePortal.Domain.Enums
{
    public enum TypeLeaveEnum
    {
        [Description("Nghỉ phép năm")]
        AnnualLeave = 1,

        [Description("Nghỉ việc riêng")]
        PersonalLeave = 2,

        [Description("Nghỉ ốm")]
        SickLeave = 3,

        [Description("Nghỉ cưới")]
        UnpaidLeave = 4,

        [Description("Nghỉ Tai nạn LĐ")]
        AccidentLeave = 5,

        [Description("Khác")]
        OtherLeave = 6
    }
}

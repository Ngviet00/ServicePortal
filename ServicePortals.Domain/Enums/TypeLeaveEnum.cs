﻿using System.ComponentModel;

namespace ServicePortals.Domain.Enums
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
        WeddingLeave = 4,

        [Description("Nghỉ Tai nạn LĐ")]
        AccidentLeave = 5,

        [Description("Khác")]
        OtherLeave = 6
    }
}

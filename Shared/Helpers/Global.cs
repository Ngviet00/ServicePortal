namespace ServicePortals.Infrastructure.Helpers
{
    public static class Global
    {
        public const string DbWeb = "ServicePortal";
        public const string DbViClock= "vs_new";
        public const string EmailDefault = "nguyenviet@vsvn.com.vn";
        public const string UserCodeSuperAdmin = "0";
        public const string DefaultExpirationDaysRefreshToken = "2";
        public const int DefaultDepartmentIdHR = 110;

        #region
        public const string CacheKeyGetAllMemoNotifyInHomePage = "get_all_memo_notify_in_homepage";

        public const string CacheKeyGetAllDepartment = "get_all_department";
        public const string CacheKeyGetAllDepartmentDistinctName = "get_all_department_distinct_name";

        public const string CacheKeyGetAllPosition = "get_all_position";

        public const string CacheKeyGetAllUserManageAttendance = "get_all_user_manage_attendance";
        #endregion
    }
}

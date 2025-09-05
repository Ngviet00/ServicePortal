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

        public const string EmailHRReceiveLeaveRequest = "nguyenviet@vsvn.com.vn";

        public const string UserNotSetInformation = "The user information has not been updated. Please contact HR.";

        public const string NotFoundApprovalFlow = "Could not found your approval flow create notification. Please contact IT.";

        public const string NotPermissionApproval = "You are not permitted to approve this request.";

        public const string HasBeenApproval = "This request has been approved, please reload page";

        public const int ParentOrgPositionGM = 1;

        #region
        public const string CacheKeyGetAllMemoNotifyInHomePage = "get_all_memo_notify_in_homepage";
        public const string CacheKeyGetAllDepartment = "get_all_department";
        public const string CacheKeyGetAllDepartmentDistinctName = "get_all_department_distinct_name";
        public const string CacheKeyGetAllPosition = "get_all_position";
        public const string CacheKeyGetAllUserManageAttendance = "get_all_user_manage_attendance";
        #endregion
    }
}

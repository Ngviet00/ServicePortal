namespace ServicePortals.Application.Common
{
    public static class TemplateEmail
    {
        public static string EmailResetPassword(string password)
        {
            return $@"
                <h2>Your Password Has Been Reset</h2>
                <div style=""font-size: 18px;"">
                    An administrator has reset your password. You can now log in using the password below: <br/>
                </div>
                <div style=""font-size: 25px;margin-top: 10px; color: #e71a1a;letter-spacing: 1px"">
                    {password}
                </div>
                <div style=""font-size: 18px;margin-top: 10px;"">
                    For security reasons, please change your password after logging in. <br/> <br/>
                    Thanks, <br/><br/>
                    MIS/IT Team
                </div>
            ";
        }
    }
}

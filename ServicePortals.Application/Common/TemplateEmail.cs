namespace ServicePortals.Application.Common
{
    public static class TemplateEmail
    {
        public static string SendContentEmail(string title, string url, string code)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""vi"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Detail</title>
                    </head>
                    <body style=""font-family: Arial, sans-serif;margin: 0; padding: 20px;"">
                        <div>
                            <h3 style=""color: #333333; text-align: left;"">{title}</h2>
                            <p style=""color: #555555; line-height: 1.6; font-size: 14px; font-weight: normal"">
                                Click here to view details:
                                <a href=""{url}"" style=""font-weight: bold; color: #007bff; text-decoration: underline;"">
                                    {code}
                                </a>
                            </p>
                        </div>
                    </body>
                </html>
            ";
        }

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

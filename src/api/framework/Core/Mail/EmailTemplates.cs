namespace FSH.Framework.Core.Mail;

public static class EmailTemplates
{
    public static class UserInvitation
    {
        public const string Subject = "You're invited to join {0}";
        
        public static string GetHtmlBody(string tenantName, string inviterName, string acceptUrl, string displayName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Invitation to join {tenantName}</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f4f4f4; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .button {{ display: inline-block; padding: 12px 30px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ background-color: #f4f4f4; padding: 20px; text-align: center; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to {tenantName}!</h1>
        </div>
        <div class='content'>
            <p>Hi {displayName},</p>
            <p>{inviterName} has invited you to join <strong>{tenantName}</strong>.</p>
            <p>To accept this invitation and create your account, please click the button below:</p>
            <div style='text-align: center;'>
                <a href='{acceptUrl}' class='button'>Accept Invitation</a>
            </div>
            <p>This invitation will expire in 7 days. If you have any questions, please contact {inviterName}.</p>
            <p>If you cannot click the button above, copy and paste this link into your browser:</p>
            <p style='word-break: break-all;'>{acceptUrl}</p>
        </div>
        <div class='footer'>
            <p>This is an automated message. Please do not reply to this email.</p>
            <p>&copy; {DateTime.UtcNow.Year} {tenantName}. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
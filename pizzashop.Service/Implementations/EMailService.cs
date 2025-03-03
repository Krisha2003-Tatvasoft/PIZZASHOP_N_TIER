using MailKit;
using MailKit.Security;
using MimeKit;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class EMailService : IEMailService
{
 
    public async Task SendResetPasswordEmail(string email,BodyBuilder body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your App Name", "test.dotnet@etatvasoft.com"));
        message.To.Add(new MailboxAddress(email, email));
        message.Subject = "Reset Your Password";

      
        message.Body = body.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        await smtp.ConnectAsync("mail.etatvasoft.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("test.dotnet@etatvasoft.com", "P}N^{z-]7Ilp");
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

   

}

using MimeKit;

namespace pizzashop.Service.Interfaces;

public interface IEMailService
{
    Task SendResetPasswordEmail(string email,BodyBuilder body);
}

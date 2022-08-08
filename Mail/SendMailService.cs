using MailKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Security;
using System.IO;
using Microsoft.Extensions.Options;

namespace webapp_examble.Mail;

public class SendMailService : IEmailSender
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<SendMailService> _logger;

    public SendMailService(IOptions<MailSettings> mailSettings, ILogger<SendMailService> logger)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = htmlMessage;
        message.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(message);
        }
        catch (Exception ex)
        {
            Directory.CreateDirectory("mail_error_save");
            var emailSave = String.Format(@"mail_error_save/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailSave);

            _logger.LogInformation(@"Send mail error, save in - {0}", emailSave);
            _logger.LogError(ex.Message);
        }

        smtp.Disconnect(true);
        
        _logger.LogInformation(@"Send mail to {0}", email);
    }
}
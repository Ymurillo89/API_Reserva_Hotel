using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

using Microsoft.Extensions.Logging;

namespace API_Hotel.Infrastructure.Messaging
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            return await EnviarCorreoHtmlAsync(destinatario, asunto, cuerpo);
        }

        public async Task<bool> EnviarCorreoHtmlAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                var smtpServer = _configuration["Smtp:Server"];
                var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
                var smtpUsername = _configuration["Smtp:Username"];
                var smtpPassword = _configuration["Smtp:Password"];
                var smtpFromEmail = _configuration["Smtp:FromEmail"];
                var smtpFromName = _configuration["Smtp:FromName"] ?? "Sistema de Reservas";
                var useTls = bool.Parse(_configuration["Smtp:UseTLS"] ?? "true");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(smtpFromName, smtpFromEmail));
                message.To.Add(new MailboxAddress("", destinatario));
                message.Subject = asunto;

                var bodyBuilder = new BodyBuilder { HtmlBody = cuerpoHtml };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, useTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                    await client.AuthenticateAsync(smtpUsername, smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Correo enviado exitosamente a {destinatario}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo a {destinatario}: {ex.Message}");
                return false;
            }
        }
    }
}

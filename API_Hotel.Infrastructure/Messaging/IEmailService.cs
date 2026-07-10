namespace API_Hotel.Infrastructure.Messaging
{
    public interface IEmailService
    {
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
        Task<bool> EnviarCorreoHtmlAsync(string destinatario, string asunto, string cuerpoHtml);
    }
}

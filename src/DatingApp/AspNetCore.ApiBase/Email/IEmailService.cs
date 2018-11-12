using AspNetCore.ApiBase.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.ApiBase.Email
{
    public interface IEmailService
    {
        Task<Result> SendEmailMessageAsync(EmailMessage message, bool sendOverride = false);
        Task<Result> SendEmailAsync(string email, string subject, string message, bool html);
        bool SendEmailMessages(IList<EmailMessage> messages);
        Task<Result> SendEmailMessageToAdminAsync(EmailMessage message);
    }
}

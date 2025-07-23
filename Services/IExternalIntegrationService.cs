using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IExternalIntegrationService
    {
        Task<string> UploadImageStreamAsync(Stream stream, string keyNameInBucket, string contentType = "image/jpeg");
        Task SendEmailAsync(string toEmail, string subject, string body, string username, string? replyToEmail = null);
    }
}

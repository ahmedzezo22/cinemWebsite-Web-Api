using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace cinemaApp.Api.Services
{
    public static class sendGrid
    {
      
    public static async Task<bool> Execute(string userEmail,string userName,string  subjectText,string Text,string Content)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ahmedelmajek2000@gmail.com", "Eng:AhmedMourad");
            var subject = subjectText;
            var to = new EmailAddress(userEmail,userName);
            var plainTextContent = Text;
            var htmlContent = Content;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return await Task.FromResult(true);
        }
    
    }
}
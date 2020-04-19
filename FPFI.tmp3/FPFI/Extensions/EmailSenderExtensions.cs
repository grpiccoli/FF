using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FPFI.Models;
using FPFI.Services;

namespace FPFI.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
        public static Task SendEmailResultsAsync(
            this IEmailSender emailSender,
            string email, string link, string name, string id)
        {
            return emailSender.SendEmailAsync(email, "FPFI Entry Results are Ready",
                $@"Dear {name}, 
The results for your entry submition number {id} can be viewed on the following link : <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}

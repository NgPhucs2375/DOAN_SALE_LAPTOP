using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace WEB_SALE_LAPTOP.Common
{
    public class MailHelper
    {
        public void SendMail(string toEmail, string subject, string content)
        {
            var fromEmailAddress = "tranphuc2375@gmail.com"; // <-- Thay Email của bạn
            var fromEmailPassword = "bkxf uiid pvag raaa";    // <-- Thay Mật khẩu ứng dụng 16 ký tự
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;

            bool enabledSsl = true;

            string body = content;
            MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, "Laptop Pro System"), new MailAddress(toEmail));
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
            client.Host = smtpHost;
            client.EnableSsl = enabledSsl;
            client.Port = smtpPort;

            client.Send(message);
        }
    }
}
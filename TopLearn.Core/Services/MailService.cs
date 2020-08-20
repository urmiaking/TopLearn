using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;

namespace TopLearn.Core.Services
{
    public class MailService : IMailService
    {
        private readonly AppDbContext _db;

        public MailService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> SendEmailAsync(Email email)
        {
            var mailServer = await _db.MailServers.FindAsync(1);

            var mail = new MailMessage();
            var smtpServer = new SmtpClient(mailServer.Host);
            mail.From = new MailAddress(mailServer.ServerAddress, "تاپ لرن");
            mail.To.Add(email.To);
            mail.Subject = email.Subject;
            mail.Body = email.Body;
            mail.IsBodyHtml = true;

            smtpServer.Port = mailServer.Port;
            smtpServer.Credentials = new System.Net.NetworkCredential(mailServer.ServerAddress, mailServer.Password);
            smtpServer.EnableSsl = true;

            try
            {
                smtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}

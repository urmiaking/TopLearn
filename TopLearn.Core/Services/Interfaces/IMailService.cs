using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TopLearn.DataLayer.Entities.Mail;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(Email email);
    }
}

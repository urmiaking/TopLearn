using System;
using System.Collections.Generic;
using System.Text;

namespace TopLearn.DataLayer.Entities.Mail
{
    public class MailServer
    {
        public int Id { get; set; }
        public string ServerAddress { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIS.Models;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;

namespace AIS
{

    public class EmailConfiguration
    {
        private readonly SessionHandler sessionHandler = new SessionHandler();
        public bool ConfigEmail()
        {
            try
            {
                return true;
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.ztbl.com.pk");

                mail.From = new MailAddress("ali.125525@ztbl.com.pk");
                mail.To.Add("khattak.aqib@ztbl.com.pk");
                mail.Subject = "Test Mail";
                mail.Body = "This is testing Email from AIS";
                SmtpServer.Host = "mail.ztbl.com.pk";
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("ali.125525@ztbl.com.pk", "PASS");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception e) { return false; }
        }

    }  
}

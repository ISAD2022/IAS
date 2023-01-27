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
        private readonly EmailCredentails emailCredentails = new EmailCredentails();
     
        public bool ConfigEmail(string to="", string cc="", string subj="", string body="")
        {
            try
            {
                //return true;
                //------WORKING CODE COMMENTED DO NOT DELETE ------

                EmailCredentailsModel em = emailCredentails.GetEmailCredentails();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(em.Host);
                mail.From = new MailAddress(em.EMAIL);
                mail.To.Add(to);
                mail.CC.Add(cc);
                mail.Subject = subj;
                mail.Body = body;
                SmtpServer.Host = em.Host;
                SmtpServer.Port = em.Port;
                SmtpServer.Credentials = new System.Net.NetworkCredential(em.EMAIL, em.PASSWORD);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception) { return false; }
        }

    }  
}

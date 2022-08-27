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
     
        public bool ConfigEmail()
        {
            try
            {
                //------WORKING CODE COMMENTED DO NOT DELETE ------

                EmailCredentailsModel em = emailCredentails.GetEmailCredentails();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(em.Host);
                mail.From = new MailAddress(em.EMAIL);
                mail.To.Add("khattak.aqib@ztbl.com.pk");
                mail.CC.Add("svp.isad@ztbl.com.pk");
                mail.Subject = "Criteria Approval Notification";
                mail.Body = "<div><b><u><span style=\"font - size:14.0pt\">Notification from Audit & amp; Inspection System<o:p ></ o:p ></ span ></ u ></ b ></ div >< div >< b >< u > (DON’T REPLY THIS EMAIL)</ u ></ b ></ div > Criteria has been approved by CIA";
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

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
using InspectionSolution.Models;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;

namespace InspectionSolution
{

    public class EmailCredentails
    {
          public EmailCredentailsModel GetEmailCredentails()
        {
            EmailCredentailsModel em = new EmailCredentailsModel();
            em.EMAIL = "noreply.audit@ztbl.com.pk";
            em.PASSWORD = "Hello@321";
            em.Host= "mail.ztbl.com.pk";
            em.Port = 587;
            return em;
           
        }

    }  
}

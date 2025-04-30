using AIS.Models;

namespace AIS
    {

    public class EmailCredentails
        {
        public EmailCredentailsModel GetEmailCredentails()
            {
            EmailCredentailsModel em = new EmailCredentailsModel();
            em.EMAIL = "noreply.audit@ztbl.com.pk";
            em.PASSWORD = "Hello@321";
            em.Host = "webmail.ztbl.com.pk";
            em.Port = 587;
            return em;

            }

        }
    }

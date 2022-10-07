using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using AIS.Models;

namespace AIS
{
    public class SessionHandler
    {
        private static SessionModel smodel = new SessionModel();
        private static DBConnection dBConnection = new DBConnection();
        public void SetSessionUser(UserModel user)
        {
            smodel.Email = user.Email;
            smodel.Name = user.Name;
            smodel.PPNumber = user.PPNumber;
            smodel.ID = user.ID;
            smodel.UserPostingAuditZone = user.UserPostingAuditZone;
            smodel.UserPostingBranch = user.UserPostingBranch;
            smodel.UserPostingDept = user.UserPostingDept;
            smodel.UserPostingDiv = user.UserPostingDiv;
            smodel.UserPostingZone = user.UserPostingZone;
            smodel.IsActive = user.IsActive;
            smodel.UserLocationType = user.UserLocationType;
            smodel.UserGroupID = user.UserGroupID;
            smodel.UserRoleID = user.UserRoleID;
        }
        public SessionModel GetSessionUser()
        {
            return smodel;
        }
        public bool DisposeUserSession()
        {
            smodel = new SessionModel();
            return true;
        }
        public bool IsUserLoggedIn()
        {
            if (dBConnection.IsLoginSessionExist())
                return true;
            else
                return false;
        }
        public bool HasPermissionToViewPage(string page_name)
        {
            bool permission = false;
            var pagesToView = dBConnection.GetTopMenuPages();
            foreach(var item in pagesToView)
            {
                if(page_name.ToLower()==(item.Page_Path.Split('/')[item.Page_Path.Split('/').Length-1]).ToLower())
                    permission = true;
            }
            if (page_name.ToLower() == "home")
                permission = true;
            return permission;
        }

    }
}

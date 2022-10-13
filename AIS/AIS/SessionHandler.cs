using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using AIS.Models;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Linq;

namespace AIS
{
    public class SessionHandler
    {
        private static List<SessionModel> sessionArr = new List<SessionModel>();
        private static SessionModel smodel = new SessionModel();
        private static DBConnection dBConnection = new DBConnection();
        private static LocalIPAddress ipaddr = new LocalIPAddress();
        public void SetSessionUser(UserModel user)
        {
            bool alreadyExists = sessionArr.Any(x => x.MACAddress == ipaddr.GetMACAddress() && x.PPNumber==user.PPNumber);
            if (!alreadyExists)
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
                smodel.SessionId = GenerateRandomCryptographicSessionKey(128);
                smodel.IPAddress = ipaddr.GetLocalIpAddress();
                smodel.MACAddress = ipaddr.GetMACAddress();
                sessionArr.Add(smodel);
            }
        }
        public SessionModel GetSessionUser()
        {
            return sessionArr.Where(x => x.MACAddress == ipaddr.GetMACAddress()).LastOrDefault();
        }
        public bool DisposeUserSession()
        {
            var itemToRemove = sessionArr.Single(r => r.MACAddress == ipaddr.GetMACAddress());
            sessionArr.Remove(itemToRemove);
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
        public string GenerateRandomCryptographicSessionKey(int keyLength=128)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

    }
}

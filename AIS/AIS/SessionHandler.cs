using AIS.Controllers;
using AIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace AIS
    {
    public class SessionHandler
        {

        private static SessionModel smodel = new SessionModel();
        private static DBConnection dBConnection;
        private static LocalIPAddress ipaddr = new LocalIPAddress();

        public ISession _session;
        public IHttpContextAccessor _httpCon;
        public IConfiguration _configuration;

        public SessionHandler()
            {

            }
        public SessionHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            {
            _session = httpContextAccessor.HttpContext.Session;
            _httpCon = httpContextAccessor;
            _configuration = configuration;
            }

        public SessionModel SetSessionUser(UserModel user)
            {
            smodel.Email = user.Email;
            smodel.Name = user.Name;
            smodel.PPNumber = user.PPNumber;
            smodel.ID = user.ID;
            smodel.UserEntityName = user.UserEntityName;
            smodel.UserRoleName = user.UserRoleName;
            smodel.UserPostingAuditZone = user.UserPostingAuditZone;
            smodel.UserPostingBranch = user.UserPostingBranch;
            smodel.UserPostingDept = user.UserPostingDept;
            smodel.UserPostingDiv = user.UserPostingDiv;
            smodel.UserPostingZone = user.UserPostingZone;
            smodel.UserEntityID = user.UserEntityID;
            smodel.IsActive = user.IsActive;
            smodel.UserLocationType = user.UserLocationType;
            smodel.UserGroupID = Convert.ToInt32(user.UserGroupID);
            smodel.UserRoleID = Convert.ToInt32(user.UserRoleID);
            smodel.SessionId = _session.Id;
            smodel.IPAddress = ipaddr.GetLocalIpAddress();
            smodel.MACAddress = ipaddr.GetMACAddress();
            smodel.FirstMACCardAddress = ipaddr.GetFirstMACCardAddress();
            _session.SetString("_sessionId", Newtonsoft.Json.JsonConvert.SerializeObject(smodel));
            return smodel;
            }
        public SessionModel GetSessionUser()
            {
            var smodel = new SessionModel();
            string json = _session.GetString("_sessionId");
            if (json != "" && json != null && json.Length > 0)
                {
                smodel = JsonSerializer.Deserialize<SessionModel>(json);
                }
            return smodel;
            }
        public bool DisposeUserSession()
            {
            _session.Clear();
            return true;
            }
        public bool IsUserLoggedIn()
            {
            dBConnection = new DBConnection();
            dBConnection._httpCon = this._httpCon;
            dBConnection._session = this._session;
            dBConnection._configuration = this._configuration;
            string json = _session.GetString("_sessionId");
            if (json != "" && json != null && json.Length > 0 && dBConnection.IsLoginSessionExist())
                {
                return true;
                }
            else
                {
                return false;
                }
            }
        public bool HasPermissionToViewPage(string page_name)
            {
            dBConnection = new DBConnection();
            dBConnection._httpCon = this._httpCon;
            dBConnection._session = this._session;
            dBConnection._configuration = this._configuration;

            bool permission = false;
            var pagesToView = dBConnection.GetTopMenuPages();
            foreach (var item in pagesToView)
                {
                if (page_name.ToLower() == (item.Page_Path.Split('/')[item.Page_Path.Split('/').Length - 1]).ToLower())
                    permission = true;
                }
            if ((page_name.ToLower() == "home") || (page_name.ToLower() == "change_password"))
                permission = true;
            return permission;
            }
        public string GenerateRandomCryptographicSessionKey(int keyLength = 128)
            {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
            }


        }
    }

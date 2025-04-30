using AIS.Controllers;
using AIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace AIS
    {

    public class TopMenus
        {
        private DBConnection dBConnection;
        private SessionHandler sessionHandler;
        public IConfiguration _configuration;
        public ISession _session;
        public IHttpContextAccessor _httpCon;

        public TopMenus(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            {
            _session = httpContextAccessor.HttpContext.Session;
            _httpCon = httpContextAccessor;
            _configuration = configuration;
            }
        public TopMenus()
            {

            }

        public List<Object> GetTopMenus()
            {

            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session; sessionHandler._configuration = this._configuration;

            dBConnection = new DBConnection();
            dBConnection._httpCon = this._httpCon;
            dBConnection._session = this._session;
            dBConnection._configuration = this._configuration;

            List<object> menuList = new List<object>();
            if (sessionHandler.IsUserLoggedIn())
                {
                var menus = dBConnection.GetTopMenus();
                foreach (var item in menus)
                    {
                    menuList.Add(item);
                    }
                }
            return menuList;
            }

        public List<Object> GetTopMenusPages()
            {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session; sessionHandler._configuration = this._configuration;


            dBConnection = new DBConnection();
            dBConnection._httpCon = this._httpCon;
            dBConnection._session = this._session;
            dBConnection._configuration = this._configuration;

            List<object> menuList = new List<object>();
            List<object> submenuList = new List<object>();
            if (sessionHandler.IsUserLoggedIn())
                {
                var menus = dBConnection.GetTopMenuPages();
                foreach (var item in menus)
                    {
                    menuList.Add(item);

                    }

                }
            var loggedInUser = sessionHandler.GetSessionUser();
            AvatarNameDisplayModel av = new AvatarNameDisplayModel();
            av.Menu_Id = 1020304050;
            av.Id = 11223344;
            av.PPNO = loggedInUser.PPNumber;
            av.User_Entity_Name = loggedInUser.UserEntityName;
            av.User_Role_Name = loggedInUser.UserRoleName;
            av.Name = loggedInUser.Name;
            av.Sub_Menu = "";
            av.Sub_Menu_Id = "";
            av.Sub_Menu_Name = "";

            menuList.Add(av);
            return menuList;
            }

        }
    }

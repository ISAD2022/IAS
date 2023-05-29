using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIS.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using AIS.Controllers;

namespace AIS
{

    public class TopMenus
    {
        private DBConnection dBConnection;
        private SessionHandler sessionHandler;

        public ISession _session;
        public IHttpContextAccessor _httpCon;

        public TopMenus(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _httpCon = httpContextAccessor;
        }
        public TopMenus()
        {

        }

        public List<Object> GetTopMenus()
        {
            
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            
            dBConnection = new DBConnection(); 
            dBConnection._httpCon = this._httpCon;
            dBConnection._session = this._session;

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
            sessionHandler._session = this._session;


            dBConnection = new DBConnection();
            dBConnection._httpCon = this._httpCon;
            dBConnection._session = this._session;

            List<object> menuList = new List<object>();
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
            av.PPNO = loggedInUser.PPNumber;
            av.Name = loggedInUser.Name;

            menuList.Add(av);
            return menuList;
        }

    }
}

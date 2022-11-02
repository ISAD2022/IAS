using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AIS.Controllers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AIS.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        

        public LoginController(ILogger<LoginController> logger, SessionHandler _sessionHandler, DBConnection _dbCon)
        {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
        }

        public IActionResult Index()
        {
            TempData["Message"] = "";
            TempData["SessionKill"] = "";
            return View();
        }
      
        public IActionResult Logout()
        {
            dBConnection.DisposeLoginSession();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        public UserModel DoLogin(LoginModel login)
        {
             var user=dBConnection.AutheticateLogin(login);
            if (user.ID != 0 && !user.isAlreadyLoggedIn && user.isAuthenticate)
            {
                return user;                
            }else
            {
                if (user.isAuthenticate && user.isAlreadyLoggedIn)
                {
                    user.ErrorMsg = "You are already loggin in System";
                    return user;
                }
                else
                {
                    user.ErrorMsg = "Incorrect UserName or Password";
                    return user;
                }
            }            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

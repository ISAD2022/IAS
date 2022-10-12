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
        private readonly DBConnection dBConnection = new DBConnection();
        private readonly SessionHandler sessionHandler = new SessionHandler();

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
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
        public ActionResult DoLogin(LoginModel login)
        {
             var user=dBConnection.AutheticateLogin(login);
            if (user.ID != 0 && !user.isAlreadyLoggedIn && user.isAuthenticate)
            {
                //Inspection User Check
                if (user.UserPostingDept == 714) 
                {
                    return RedirectToAction("Home", "Inspection");
                }
                else {
                    return RedirectToAction("Index", "Home");
                }
                
            }else
            {
                if (user.isAuthenticate && user.isAlreadyLoggedIn)
                {
                    TempData["Message"] = string.Format("You are already loggin in System");
                    TempData["SessionKill"] = "killsession";
                    return View("Index", "Login");
                }
                else
                {
                    TempData["SessionKill"] = string.Format("");
                    TempData["Message"] = string.Format("Incorrect UserName or Password");
                    return View("Index", "Login");
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

using AIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AIS.Controllers
    {

    public class LoginController : Controller
        {
        private readonly ILogger<LoginController> _logger;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<LoginController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, IConfiguration configuration)
            {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            _configuration = configuration;
            }

        public IActionResult Index()
            {
            TempData["Message"] = "";
            TempData["SessionKill"] = "";
            string secretValue = _configuration["SecretKey"];
            string baseURL = _configuration["BaseURL"];
            ViewBag.SecretValue = secretValue;
            ViewBag.BaseURL = baseURL;
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

            var user = dBConnection.AutheticateLogin(login);
            if (user.ID != 0 && !user.isAlreadyLoggedIn && user.isAuthenticate)
                {
                return user;
                }
            else
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

        public string ResetPassword(string PPNumber, string CNICNumber)
            {
            return "{\"Status\":true,\"Message\":\"" + dBConnection.ResetUserPassword(PPNumber, CNICNumber) + "\"}";

            }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }

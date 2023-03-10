using AIS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;


namespace AIS.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly ILogger<DatabaseController> _logger;
        private readonly TopMenus tm;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        public DatabaseController(ILogger<DatabaseController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, TopMenus _tpMenu)
        {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            tm = _tpMenu;
        }
      
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

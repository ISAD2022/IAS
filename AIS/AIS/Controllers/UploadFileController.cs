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
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting.Server;

namespace AIS.Controllers
{

    public class UploadFileController : Controller
    {
        private readonly ILogger<UploadFileController> _logger;
        private readonly SessionHandler sessionHandler;
        private readonly DBConnection dBConnection;
        private readonly IConfiguration _configuration;

        public UploadFileController(ILogger<UploadFileController> logger, SessionHandler _sessionHandler, DBConnection _dbCon, IConfiguration configuration)
        {
            _logger = logger;
            sessionHandler = _sessionHandler;
            dBConnection = _dbCon;
            _configuration = configuration;
        }

        [HttpPost]
        [Obsolete]
        public ActionResult UploadFiles(List<ProductImage> files)
        {
            try
            {
                foreach (var file in files)
                {
                    dBConnection.SaveImage(file.IMAGE_DATA, file.FILE_NAME);
                    
                }
                return Json(new { success = true, message = "Files uploaded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Obsolete]
        public ActionResult DeleteFile(string fileName)
        {
            try
            {
                dBConnection.DeleteImage(fileName);
                return Json(new { success = true, message = "File deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
public class ProductImage
{
    public string OBS_ID { get; set; }
    public string OBS_TEXT_ID { get; set; }
    public string IMAGE_NAME { get; set; }
    public string FILE_NAME { get; set; }
    public string IMAGE_DATA { get; set; }
    public string IMAGE_TYPE { get; set; }
    public int LENGTH { get; set; }
    public int SEQUENCE { get; set; }
}
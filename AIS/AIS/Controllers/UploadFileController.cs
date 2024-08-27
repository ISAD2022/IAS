using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using AIS.Models;
using System.Diagnostics;

namespace AIS.Controllers
{
    public class UploadFileController : Controller
    {
        private readonly ILogger<UploadFileController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _uploadPath;

        public UploadFileController(ILogger<UploadFileController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            // Set the directory path where files will be uploaded
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/PostCompliance_Evidences");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
        {
            try
            {
                var subfolder = Request.Form["subfolder"];
                var uploadPath = Path.Combine(_uploadPath, subfolder);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }

                return Json(new { success = true, message = "Files uploaded successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading files");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteFile(string subFolder,string fileName)
        {
            try
            {
                var uploadPath = Path.Combine(_uploadPath, subFolder);
                var filePath = Path.Combine(uploadPath, fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    // Optionally, delete file metadata from the database
                    // Example: dBConnection.DeleteFileMetadata(fileName);
                    return Json(new { success = true, Message = "File deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, Message = "File not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<List<AuditeeResponseEvidenceModel>> GetFilesData(string subfolder)
        {
            try
            {
                var filesData = new List<AuditeeResponseEvidenceModel>();
                var folderPath = Path.Combine(_uploadPath, subfolder);
                if (!Directory.Exists(folderPath))
                {
                    return filesData;
                }

             
                var files = Directory.GetFiles(folderPath);

                foreach (var filePath in files)
                {
                    var fileName = Path.GetFileName(filePath);
                    var fileType = Path.GetExtension(filePath).TrimStart('.'); // Get the file extension without the dot
                    var fileLength = new FileInfo(filePath).Length;

                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await fileStream.CopyToAsync(memoryStream);
                            var base64String = Convert.ToBase64String(memoryStream.ToArray());

                            filesData.Add(new AuditeeResponseEvidenceModel
                            {
                                FILE_NAME = fileName,
                                IMAGE_LENGTH = Convert.ToInt64(fileLength),
                                IMAGE_TYPE = fileType,
                                IMAGE_DATA = base64String
                            });
                        }
                    }
                }

                return filesData;
            }
            catch (Exception ex)
            {
                var filesData = new List<AuditeeResponseEvidenceModel>();
                _logger.LogError(ex, "Error retrieving file data");
                return filesData;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

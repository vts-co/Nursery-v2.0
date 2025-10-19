using NurseryProject.Authorization;
using NurseryProject.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "0")]

    public class BackUpRestoreDataController : Controller
    {
        SettingsServices settingsServices = new SettingsServices();

        // GET: BackUpRestoreData
        public ActionResult BackUp()
        {
            return View();
        }

        public ActionResult Restore()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Restore(HttpPostedFileBase backupFile)
        {
            if (backupFile == null || backupFile.ContentLength == 0)
            {
                return Content("No file selected.");
            }

            // Ensure the file is a .bak file
            if (Path.GetExtension(backupFile.FileName).ToLower() != ".bak")
            {
                return Content("Please select a valid .bak file.");
            }

            // Define the backup file path
            var dbPath = Server.MapPath(@"~\DataBackUp\DBBackup.bak");

            string backupFilePath = Path.Combine(@"C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\Backup", backupFile.FileName);

            // Save the file temporarily on the server
            backupFile.SaveAs(backupFilePath);

            //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Call the service to restore the database
            settingsServices.Restore(backupFilePath);
            // Optionally, delete the temporary file after the restore process
            if (System.IO.File.Exists(backupFilePath))
            {
                System.IO.File.Delete(backupFilePath);
            }
            TempData["success"] = "تم ارجاع النسخة الاحطياطية";


            return View();
            
           
        }

    }
}
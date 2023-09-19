using NurseryProject.Authorization;
using NurseryProject.Dtos.HomePage;
using NurseryProject.Enums;
using NurseryProject.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using System.Web.UI.WebControls;
using Microsoft.Data.SqlClient;
using System.IO;
using ExcelDataReader;
using System.Data;
using NurseryProject.Models;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "0")]

    public class HomeController : Controller
    {

        SettingsServices settingsServices = new SettingsServices();
        public ActionResult Index()
        {
            List<HomePagesDro> homePages = new List<HomePagesDro>();
            var homeScreens = settingsServices.GetAllHomePages();
            var userPages = ViewBag.UserScreens;

            foreach (var item in homeScreens)
            {
                var screenId = "," + item.PageId + ",";

                if (userPages.Contains(screenId))
                {
                    var page = settingsServices.GetPage(item.PageId.Value);
                    if(page!=null)
                    {
                        homePages.Add(new HomePagesDro
                        {
                            Id = page.Id,
                            Name = page.Name,
                            Icon = page.Icone,
                            Link = page.Link
                        });
                    }
                   

                }
            }
            return View(homePages);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult BackUpDatabase()
        {
            var dbPath = Server.MapPath(@"~\DataBackUp\DBBackup.bak");
            settingsServices.BackUp(dbPath);
            string fullPath = Path.Combine(Server.MapPath("~/DataBackUp"), "DBBackup.bak");
            return File(fullPath, "application/octet-stream", "DBBackup.bak");

        }

        public ActionResult RestorDatabase()
        {
            var dbPath = Server.MapPath(@"~\DataBackUp\DBBackup.bak");
            settingsServices.Restore(dbPath);
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult DeleteAllFromDatabase(HttpPostedFileBase upload)
        {
            var dbPath = Server.MapPath(@"~\DataBackUp\DBBackup.bak");
            settingsServices.BackUp(dbPath);

            settingsServices.DeleteAll();
            string fullPath = Path.Combine(Server.MapPath("~/Pages"), "PagesExcel.xlsx");

            if (upload != null && upload.ContentLength > 0)
            {
                // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                // to get started. This is how we avoid dependencies on ACE or Interop:
                Stream stream = upload.InputStream;

                IExcelDataReader reader = null;


                if (upload.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (upload.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else
                {
                    TempData["warning"] = "البيانات المدخلة غير صحيحة";
                    return RedirectToAction("Index");
                }
                int fieldcount = reader.FieldCount;
                int rowcount = reader.RowCount;
                DataTable model = new DataTable();
                DataRow row;
                DataTable dt_ = new DataTable();
                try
                {
                    dt_ = reader.AsDataSet().Tables[0];
                    for (int i = 0; i < dt_.Columns.Count; i++)
                    {
                        var ss = dt_.Rows[0][i].ToString();
                        model.Columns.Add(dt_.Rows[0][i].ToString());
                    }
                    int rowcounter = 0;
                    for (int row_ = 1; row_ < rowcount; row_++)
                    {
                        row = model.NewRow();
                        for (int col = 0; col < fieldcount; col++)
                        {
                            row[col] = dt_.Rows[row_][col].ToString();
                            var sss = dt_.Rows[row_][col].ToString();

                            rowcounter++;
                        }
                        model.Rows.Add(row);

                    }

                }
                catch (Exception ex)
                {
                    TempData["warning"] = "البيانات المدخلة غير صحيحة";
                    return RedirectToAction("Index");
                }

                reader.Close();
                reader.Dispose();
                for (int i = 0; i < model.Rows.Count; i++)
                {
                    var userId = Guid.Parse("7C9E6679-7425-40DE-944B-E07FC1F90AE7");

                    Page page = new Page();

                    page.Name = model.Rows[i][1].ToString();
                    page.Icone = model.Rows[i][2].ToString();
                    page.Link = model.Rows[i][3].ToString();
                    if (model.Rows[i][4].ToString() != "NULL")
                        page.ParentId = int.Parse(model.Rows[i][4].ToString());
                    page.DisplayOrder = int.Parse(model.Rows[i][5].ToString());

                    page.CreatedOn = DateTime.Now;
                    page.CreatedBy = userId;
                    settingsServices.AddNewPage(page, (Guid)TempData["UserId"]);

                }
                TempData["success"] = "تم حفظ البيانات بنجاح";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = "البيانات المدخلة غير صحيحة";
                return RedirectToAction("Index");
            }
        }

        #region Import MCQ Question Excel

        public ActionResult ImportExcel(HttpPostedFileBase upload)
        {

            return View();
        }

        #endregion
    }
}
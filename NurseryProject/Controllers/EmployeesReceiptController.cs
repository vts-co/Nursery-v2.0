using NurseryProject.Authorization;
using NurseryProject.Dtos.EmployeesReceipt;
using NurseryProject.Enums;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesReceipt;
using NurseryProject.Services.Settings;
using NurseryProject.Services.StudyPlaces;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "37")]

    public class EmployeesReceiptController : Controller
    {
        SettingsServices settingsServices = new SettingsServices();
        DepartmentsServices departmentsServices = new DepartmentsServices();
        EmployeesServices employeesServices = new EmployeesServices();
        EmployeesReceiptServices employeesReceiptServices = new EmployeesReceiptServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        // GET: Cities
       
        public ActionResult Index()
        {
            var employeesModel = employeesReceiptServices.GetAllEmployeesReceipts();

            return View(employeesModel);
        }
        public ActionResult Create()
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name");

            return View("Upsert");
        }
        [HttpPost]
        public ActionResult Create(string date, Guid EmployeeId)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (date != null)
                mon = DateTime.Parse(date).ToString("yyyy-MM");

            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;

            if (model.Reciept == "0")
            {
                ViewBag.StudyYears= studyYearsServices.GetAll();
                ViewBag.StudyPlaces = studyPlacesServices.GetAll();
            }
            else
            {
                ViewBag.StudyYears = model.StudyYearName;
                ViewBag.StudyPlaces = model.StudyPlaceName;
            }
            return View("Upsert");
        }

        public ActionResult Delete(Guid Id)
        {
            var result = employeesReceiptServices.Delete(Id, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["warning"] = result.Message;
            }
            return RedirectToAction("Index");

        }

        public ActionResult Collect(string Date,string Month, Guid EmployeeId, Guid StudyYearId, Guid StudyPlaceId, float Total, float TotalDiscountCost, float TotalIncreasesCost, float Final)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (Month != null)
                mon = DateTime.Parse(Month).ToString("yyyy-MM");

            var year = DateTime.Now.ToString("yyyy-MM-ddy");
            if (Date != null)
                year = DateTime.Parse(Date).ToString("yyyy-MM-dd");

            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;
            ViewBag.date = mon;

            var result = employeesReceiptServices.Create(year, mon, EmployeeId, StudyYearId, StudyPlaceId, Total, TotalDiscountCost, TotalIncreasesCost, Final, (Guid)TempData["UserId"]);
            if(result.IsSuccess)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["warning"] = result.Message;
            }
            return RedirectToAction("Index");

        }
        
        public ActionResult DeleteCollect(string Month, Guid EmployeeId)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (Month != null)
                mon = DateTime.Parse(Month).ToString("yyyy-MM");

            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;

            ViewBag.date = mon;

            var result = employeesReceiptServices.DeleteCollect(mon, EmployeeId,(Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["warning"] = result.Message;
            }
            return RedirectToAction("Index");

        }

        [Authorized(ScreenId = "64")]
        public ActionResult Reports()
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name");
            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "64")]
        public ActionResult Reports(string date, Guid EmployeeId)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (date != null)
                mon = DateTime.Parse(date).ToString("yyyy-MM");



            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;

            if (model.Reciept != "0")
            {
                ViewBag.StudyYears = model.StudyYearName;
                ViewBag.StudyPlaces = model.StudyPlaceName;
            }

            return View();
        }

        [Authorized(ScreenId = "64")]
        public ActionResult CollectReport(string Date, string Month, Guid EmployeeId, Guid StudyYearId, Guid StudyPlaceId, float Total, float TotalDiscountCost, float TotalIncreasesCost, float Final)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (Month != null)
                mon = DateTime.Parse(Month).ToString("yyyy-MM");

            var year = DateTime.Now.ToString("yyyy-MM-ddy");
            if (Date != null)
                year = DateTime.Parse(Date).ToString("yyyy-MM-dd");

            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;
            ViewBag.date = mon;

            var result = employeesReceiptServices.Create(year, mon, EmployeeId, StudyYearId, StudyPlaceId, Total, TotalDiscountCost, TotalIncreasesCost, Final, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["warning"] = result.Message;
            }
            return RedirectToAction("Reports");

        }

        [Authorized(ScreenId = "65")]
        public ActionResult MonthlyReports()
        {
            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "65")]
        public ActionResult MonthlyReports(string date)
        {
            var date1 = DateTime.Parse(date).ToString("yyyy-MM");
            var employeesModel = employeesReceiptServices.GetAllEmployeesReceipts().Where(x => x.Month == date1).ToList();

            ViewBag.EmployeesReceipt = employeesModel;

            return View();
        }

        [Authorized(ScreenId = "65")]
        public ActionResult CollectMonthlyReport(string Date, string Month, Guid EmployeeId, Guid StudyYearId, Guid StudyPlaceId, float Total, float TotalDiscountCost, float TotalIncreasesCost, float Final)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (Month != null)
                mon = DateTime.Parse(Month).ToString("yyyy-MM");

            var year = DateTime.Now.ToString("yyyy-MM-ddy");
            if (Date != null)
                year = DateTime.Parse(Date).ToString("yyyy-MM-dd");

            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;
            ViewBag.date = mon;

            var result = employeesReceiptServices.Create(year, mon, EmployeeId, StudyYearId, StudyPlaceId, Total, TotalDiscountCost, TotalIncreasesCost, Final, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
            }
            else
            {
                TempData["warning"] = result.Message;
            }
            return RedirectToAction("Reports");

        }

        public ActionResult getEmployees(Guid Id)
        {
            if (Id == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var model = employeesServices.GetAll().Where(x => x.DepartmentId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getReportDesign()
        {
                        
            var model = settingsServices.GetAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}
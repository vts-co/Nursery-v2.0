using NurseryProject.Authorization;
using NurseryProject.Dtos.EmployeesReceipt;
using NurseryProject.Enums;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesReceipt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeesReceiptController : Controller
    {
        DepartmentsServices departmentsServices = new DepartmentsServices();
        EmployeesServices employeesServices = new EmployeesServices();
        EmployeesReceiptServices employeesReceiptServices = new EmployeesReceiptServices();
        // GET: Cities
        public ActionResult Index()
        {
           
            var employeesModel = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult Index(string date, Guid EmployeeId)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (date != null)
                mon = DateTime.Parse(date).ToString("yyyy-MM");

            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;

            return View();
        }
        public ActionResult Collect(string Date,string Month, Guid EmployeeId, float Total, float TotalDiscountCost, float TotalIncreasesCost, float Final)
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

            var result = employeesReceiptServices.Create(year, mon, EmployeeId, Total, TotalDiscountCost, TotalIncreasesCost, Final, (Guid)TempData["UserId"]);
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

            var result = employeesReceiptServices.Delete(mon, EmployeeId,(Guid)TempData["UserId"]);
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
        public ActionResult Reports()
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Reports(string date, Guid EmployeeId)
        {
            var mon = DateTime.Now.ToString("yyyy-MM");
            if (date != null)
                mon = DateTime.Parse(date).ToString("yyyy-MM");

            var employeesModel = employeesServices.GetAll().ToList();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", EmployeeId);

            var model = employeesReceiptServices.GetAll(mon, EmployeeId);
            ViewBag.EmployeesReceipt = model;

            return View();
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
    }
}
using NurseryProject.Authorization;
using NurseryProject.Dtos.EmployeeReports;
using NurseryProject.Enums;
using NurseryProject.Services.EmployeeReports;
using NurseryProject.Services.EmployeeReportTools;
using NurseryProject.Services.EmployeeReportTypes;
using NurseryProject.Services.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "85")]

    public class EmployeeReportsController : Controller
    {
        EmployeeReportToolsServices employeeReportToolsServices = new EmployeeReportToolsServices();
        EmployeeReportTypesServices employeeReportTypesServices = new EmployeeReportTypesServices();
        EmployeeReportsServices employeeReportsServices = new EmployeeReportsServices();
        EmployeesServices employeesServices = new EmployeesServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeeReportsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }

        public ActionResult Create()
        {
            var Types = employeeReportsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name");

            var employees = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeId = new SelectList(employees, "Id", "Name");

            ViewBag.DateFrom = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.DateTo = DateTime.Now.ToString("yyyy-MM-dd");

            return View("Upsert", new EmployeeReportsDto());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeeReportsDto employeeReports)
        {
            employeeReports.Id = Guid.NewGuid();
            var result = employeeReportsServices.Create(employeeReports, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeeReports.Id = Guid.Empty;

                var Types = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeeReports.EmployeeReportTypeId);

                var Students = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeId = new SelectList(Students, "Id", "Name", employeeReports.EmployeeId);

                ViewBag.DateFrom = employeeReports.ReportDateFrom.ToString("yyyy-MM-dd");
                ViewBag.DateTo = employeeReports.ReportDateTo.ToString("yyyy-MM-dd");

                TempData["warning"] = result.Message;
                return View("Upsert", employeeReports);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeeReports = employeeReportsServices.Get(Id);

            var Types = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeeReports.EmployeeReportTypeId);

            var Students = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeId = new SelectList(Students, "Id", "Name", employeeReports.EmployeeId);

            ViewBag.DateFrom = employeeReports.ReportDateFrom.ToString("yyyy-MM-dd");
            ViewBag.DateTo = employeeReports.ReportDateTo.ToString("yyyy-MM-dd");


            return View("Upsert", employeeReports);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeeReportsDto employeeReports)
        {

            var result = employeeReportsServices.Edit(employeeReports, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var Types = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeeReports.EmployeeReportTypeId);

                var Students = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeId = new SelectList(Students, "Id", "Name", employeeReports.EmployeeId);

                ViewBag.DateFrom = employeeReports.ReportDateFrom.ToString("yyyy-MM-dd");
                ViewBag.DateTo = employeeReports.ReportDateTo.ToString("yyyy-MM-dd");

                TempData["warning"] = result.Message;
                return View("Upsert", employeeReports);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeeReportsServices.Delete(Id, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult getTools(Guid Id)
        {
            var model = employeeReportToolsServices.GetAllByEmployeeReportTypeId(Id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
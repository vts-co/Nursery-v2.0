using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentReports;
using NurseryProject.Enums;
using NurseryProject.Services.StudentReports;
using NurseryProject.Services.StudentReportTools;
using NurseryProject.Services.StudentReportTypes;
using NurseryProject.Services.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "84")]
    public class StudentReportsController : Controller
    {
        StudentReportToolsServices studentReportToolsServices = new StudentReportToolsServices();
        StudentReportTypesServices studentReportTypesServices = new StudentReportTypesServices();
        StudentReportsServices studentReportsServices = new StudentReportsServices();
        StudentsServices studentsServices = new StudentsServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = studentReportsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }

        public ActionResult Create()
        {
            var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentReportTypeId = new SelectList(Types, "Id", "Name");

            var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");

            ViewBag.DateFrom = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.DateTo = DateTime.Now.ToString("yyyy-MM-dd");

            return View("Upsert", new StudentReportsDto());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudentReportsDto employeeReports)
        {
            employeeReports.Id = Guid.NewGuid();
            var result = studentReportsServices.Create(employeeReports, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeeReports.Id = Guid.Empty;

                var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudentReportTypeId = new SelectList(Types, "Id", "Name", employeeReports.StudentReportTypeId);

                var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", employeeReports.StudentId);

                ViewBag.DateFrom = employeeReports.ReportDateFrom.ToString("yyyy-MM-dd");
                ViewBag.DateTo = employeeReports.ReportDateTo.ToString("yyyy-MM-dd");

                TempData["warning"] = result.Message;
                return View("Upsert", employeeReports);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeeReports = studentReportsServices.Get(Id);

            var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentReportTypeId = new SelectList(Types, "Id", "Name", employeeReports.StudentReportTypeId);

            var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(Students, "Id", "Name", employeeReports.StudentId);

            ViewBag.DateFrom = employeeReports.ReportDateFrom.ToString("yyyy-MM-dd");
            ViewBag.DateTo = employeeReports.ReportDateTo.ToString("yyyy-MM-dd");


            return View("Upsert", employeeReports);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudentReportsDto employeeReports)
        {

            var result = studentReportsServices.Edit(employeeReports, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudentReportTypeId = new SelectList(Types, employeeReports.StudentReportTypeId);

                var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudentId = new SelectList(Students, employeeReports.StudentId);

                ViewBag.DateFrom = employeeReports.ReportDateFrom.ToString("yyyy-MM-dd");
                ViewBag.DateTo = employeeReports.ReportDateTo.ToString("yyyy-MM-dd");

                TempData["warning"] = result.Message;
                return View("Upsert", employeeReports);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studentReportsServices.Delete(Id, (Guid)TempData["UserId"]);
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
            var model = studentReportToolsServices.GetAllByStudentReportTypeId(Id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
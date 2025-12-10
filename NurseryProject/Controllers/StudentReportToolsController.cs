using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentReportTools;
using NurseryProject.Enums;
using NurseryProject.Services.StudentReportTools;
using NurseryProject.Services.StudentReportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "83")]

    public class StudentReportToolsController : Controller
    {
        StudentReportToolsServices studentReportToolsServices = new StudentReportToolsServices();
        StudentReportTypesServices studentReportTypesServices = new StudentReportTypesServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = studentReportToolsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }

        public ActionResult Create()
        {

            var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name");

            return View("Upsert", new StudentReportToolsDto());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudentReportToolsDto employeesDiscount)
        {
            employeesDiscount.Id = Guid.NewGuid();
            var result = studentReportToolsServices.Create(employeesDiscount, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesDiscount.Id = Guid.Empty;

                var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeesDiscount.StudentReportTypeId);

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDiscount);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeeReportTools = studentReportToolsServices.Get(Id);

            var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeeReportTools.StudentReportTypeId);


            return View("Upsert", employeeReportTools);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudentReportToolsDto employeesDiscount)
        {

            var result = studentReportToolsServices.Edit(employeesDiscount, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {

                var Types = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeesDiscount.StudentReportTypeId);

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDiscount);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studentReportToolsServices.Delete(Id, (Guid)TempData["UserId"]);
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
    }
}
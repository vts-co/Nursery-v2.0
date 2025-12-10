using NurseryProject.Authorization;
using NurseryProject.Dtos.EmployeeReportTools;
using NurseryProject.Enums;
using NurseryProject.Services.EmployeeReportTools;
using NurseryProject.Services.EmployeeReportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "82")]

    public class EmployeeReportToolsController : Controller
    {
        EmployeeReportToolsServices employeeReportToolsServices = new EmployeeReportToolsServices();
        EmployeeReportTypesServices employeeReportTypesServices = new EmployeeReportTypesServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeeReportToolsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }

        public ActionResult Create()
        {
         
            var Types = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name");

            return View("Upsert", new EmployeeReportToolsDto());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeeReportToolsDto employeesDiscount)
        {
            employeesDiscount.Id = Guid.NewGuid();
            var result = employeeReportToolsServices.Create(employeesDiscount, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesDiscount.Id = Guid.Empty;

                var Types = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeReportTypeId = new SelectList(Types,"Id","Name", employeesDiscount.EmployeeReportTypeId);

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDiscount);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeeReportTools = employeeReportToolsServices.Get(Id);

            var Types = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeeReportTools.EmployeeReportTypeId);


            return View("Upsert", employeeReportTools);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeeReportToolsDto employeesDiscount)
        {

            var result = employeeReportToolsServices.Edit(employeesDiscount, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {

                var Types = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeReportTypeId = new SelectList(Types, "Id", "Name", employeesDiscount.EmployeeReportTypeId);

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDiscount);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeeReportToolsServices.Delete(Id, (Guid)TempData["UserId"]);
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
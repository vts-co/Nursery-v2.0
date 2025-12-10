using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.EmployeeReportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "80")]

    public class EmployeeReportTypesController : Controller
    {
        EmployeeReportTypesServices employeeReportTypesServices = new EmployeeReportTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new EmployeeReportType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeeReportType employeeReportType)
        {
            employeeReportType.Id = Guid.NewGuid();
            var result = employeeReportTypesServices.Create(employeeReportType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeeReportType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", employeeReportType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var discountsType = employeeReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", discountsType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeeReportType employeeReportType)
        {

            var result = employeeReportTypesServices.Edit(employeeReportType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", employeeReportType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeeReportTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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
using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.StudentReportTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "81")]

    public class StudentReportTypesController : Controller
    {
        StudentReportTypesServices studentReportTypesServices = new StudentReportTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new StudentReportType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudentReportType studentReportType)
        {
            studentReportType.Id = Guid.NewGuid();
            var result = studentReportTypesServices.Create(studentReportType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                studentReportType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", studentReportType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var discountsType = studentReportTypesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", discountsType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudentReportType studentReportType)
        {

            var result = studentReportTypesServices.Edit(studentReportType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", studentReportType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studentReportTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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
using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class StudyYearsController : Controller
    {
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = studyYearsServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new StudyYear());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudyYear studyYear)
        {
            studyYear.Id = Guid.NewGuid();
            if (studyYear.DisplayOrder == null)
                studyYear.DisplayOrder = 0;
            var result = studyYearsServices.Create(studyYear, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                studyYear.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", studyYear);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var studyYear = studyYearsServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", studyYear);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudyYear studyYear)
        {
            if (studyYear.DisplayOrder == null)
                studyYear.DisplayOrder = 0;
            var result = studyYearsServices.Edit(studyYear, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", studyYear);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studyYearsServices.Delete(Id, (Guid)TempData["UserId"]);
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
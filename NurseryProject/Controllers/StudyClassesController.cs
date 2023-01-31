using NurseryProject.Authorization;
using NurseryProject.Dtos.StudyClasses;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.StudyClasses;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class StudyClassesController : Controller
    {
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyClassesServices studyClasses = new StudyClassesServices();

        // GET: Destricts
        public ActionResult Index()
        {

            var model = studyClasses.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyYearsModel = studyYearsServices.GetAll();
            ViewBag.StudyYears = studyYearsModel;

            return View("Upsert", new StudyClass());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudyClass studyClass)
        {
            studyClass.Id = Guid.NewGuid();
            var result = studyClasses.Create(studyClass, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                studyClass.Id = Guid.Empty;

                var studyYearsModel = studyYearsServices.GetAll();
                ViewBag.StudyYears = studyYearsModel;

                TempData["warning"] = result.Message;
                return View("Upsert", studyClass);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var studyYearsModel = studyYearsServices.GetAll();
            ViewBag.StudyYears = studyYearsModel;

            var studyClass = studyClasses.Get(Id);
            return View("Upsert", studyClass);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudyClass studyClass)
        {

            var result = studyClasses.Edit(studyClass, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var studyYearsModel = studyYearsServices.GetAll();
                ViewBag.StudyYears = studyYearsModel;

                TempData["warning"] = result.Message;
                return View("Upsert", studyClass);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studyClasses.Delete(Id, (Guid)TempData["UserId"]);
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
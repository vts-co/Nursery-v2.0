using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "16")]

    public class SubjectsController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        SubjectsServices subjectsServices = new SubjectsServices();
        // GET: Destricts
        public ActionResult Index()
        {

            var model = subjectsServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");

           
            return View("Upsert", new Subject());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Subject subject)
        {
            subject.Id = Guid.NewGuid();
            var result = subjectsServices.Create(subject, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                subject.Id = Guid.Empty;

                var level1 = levelsServices.Get(subject.LevelId.Value);
                var StudyTypeId = level1.StudyTypeId;

                var studyTypes = studyTypesServices.GetAll();
                var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == level1.StudyTypeId).ToList();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level1.StudyTypeId);
                ViewBag.LevelId = new SelectList(levels, "Id", "Name", subject.LevelId);

                
                TempData["warning"] = result.Message;
                return View("Upsert", subject);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var subject = subjectsServices.Get(Id);

           
            var level1 = levelsServices.Get(subject.LevelId.Value);
            var StudyTypeId = level1.StudyTypeId;

            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);

            var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();
            ViewBag.LevelId = new SelectList(levels, "Id", "Name", subject.LevelId.Value);

            return View("Upsert", subject);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Subject subject)
        {

            var result = subjectsServices.Edit(subject, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var level1 = levelsServices.Get(subject.LevelId.Value);
                var StudyTypeId = level1.StudyTypeId;

                var studyTypes = studyTypesServices.GetAll();
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);

                var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();
                ViewBag.LevelId = new SelectList(levels, "Id", "Name", subject.LevelId);


                TempData["warning"] = result.Message;
                return View("Upsert", subject);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = subjectsServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult getLevels(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
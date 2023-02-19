using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.StudyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "14")]

    public class LevelsController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();

        // GET: Destricts
        public ActionResult Index()
        {

            var model = levelsServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var cityModel = studyTypesServices.GetAll();
            ViewBag.studyTypes = cityModel;

            return View("Upsert", new Level());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Level level)
        {
            level.Id = Guid.NewGuid();
            if (level.DisplayOrder == null)
                level.DisplayOrder = 0;
            var result = levelsServices.Create(level, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                level.Id = Guid.Empty;

                var studyTypeModel = studyTypesServices.GetAll();
                ViewBag.studyTypes = studyTypeModel;

                TempData["warning"] = result.Message;
                return View("Upsert", level);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var studyTypeModel = studyTypesServices.GetAll();
            ViewBag.studyTypes = studyTypeModel;

            var level = levelsServices.Get(Id);
            return View("Upsert", level);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Level level)
        {
            if (level.DisplayOrder == null)
                level.DisplayOrder = 0;
            var result = levelsServices.Edit(level, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var studyTypeModel = studyTypesServices.GetAll();
                ViewBag.studyTypes = studyTypeModel;

                TempData["warning"] = result.Message;
                return View("Upsert", level);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = levelsServices.Delete(Id, (Guid)TempData["UserId"]);
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
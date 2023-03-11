using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.StudyPlaces;
using NurseryProject.Services.StudyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "15")]

    public class ClassesController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        // GET: Destricts
        public ActionResult Index()
        {

            var model = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }

        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id","Name");

            ViewBag.LevelId = new SelectList("");

            var studyPlaces = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudyPlaces = studyPlaces;

            return View("Upsert", new Class());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Class Class)
        {
            Class.Id = Guid.NewGuid();
            var result = classesServices.Create(Class, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                Class.Id = Guid.Empty;

                var level1 = levelsServices.Get(Class.LevelId.Value);
                var StudyTypeId = level1.StudyTypeId;

                var studyTypes = studyTypesServices.GetAll();
                var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);
                ViewBag.LevelId = new SelectList(levels, "Id", "Name", Class.LevelId.Value);

                var studyPlaces = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudyPlaces = studyPlaces;

                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var class1 = classesServices.Get(Id);

            var studyPlaces = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudyPlaces = studyPlaces;

            var level1 = levelsServices.Get(class1.LevelId.Value);
            var StudyTypeId = level1.StudyTypeId;

            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);

            var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();
            ViewBag.LevelId = new SelectList(levels, "Id", "Name", class1.LevelId.Value);

            return View("Upsert", class1);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Class Class)
        {

            var result = classesServices.Edit(Class, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var level1 = levelsServices.Get(Class.LevelId.Value);
                var StudyTypeId = level1.StudyTypeId;

                var studyPlaces = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudyPlaces = studyPlaces;

                var studyTypes = studyTypesServices.GetAll();
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);

                var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();
                ViewBag.LevelId = new SelectList(levels, "Id", "Name", Class.LevelId);


                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = classesServices.Delete(Id, (Guid)TempData["UserId"]);
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
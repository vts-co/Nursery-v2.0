using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.StudyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class StudyTypesController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = studyTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new StudyType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudyType studyType)
        {
            studyType.Id = Guid.NewGuid();
            var result = studyTypesServices.Create(studyType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                studyType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", studyType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var studyType = studyTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", studyType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudyType studyType)
        {

            var result = studyTypesServices.Edit(studyType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", studyType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studyTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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
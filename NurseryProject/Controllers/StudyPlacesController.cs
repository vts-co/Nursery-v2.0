using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.StudyPlaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "13")]

    public class StudyPlacesController : Controller
    {
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new StudyPlace());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudyPlace studyPlace)
        {
            studyPlace.Id = Guid.NewGuid();
            var result = studyPlacesServices.Create(studyPlace, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                studyPlace.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", studyPlace);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var studyPlace = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", studyPlace);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudyPlace studyPlace)
        {

            var result = studyPlacesServices.Edit(studyPlace, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", studyPlace);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studyPlacesServices.Delete(Id, (Guid)TempData["UserId"]);
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
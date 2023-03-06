using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Employees;
using NurseryProject.Services.Revenues;
using NurseryProject.Services.RevenuesTypes;
using NurseryProject.Services.StudyPlaces;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "43")]
    public class RevenuesController : Controller
    {
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();

        RevenuesServices revenuesServices = new RevenuesServices();
        RevenuesTypesServices revenuesTypesServices = new RevenuesTypesServices();
        EmployeesServices employeesServices = new EmployeesServices();
        ClassesServices classesServices = new ClassesServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = revenuesServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyPlaces = studyPlacesServices.GetAll();
            ViewBag.StudyPlaces = studyPlaces;

            var studyYears = studyYearsServices.GetAll();
            ViewBag.StudyYears = studyYears;

            var RevenueTypeParentIdModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
            ViewBag.RevenueTypeParentId = new SelectList(RevenueTypeParentIdModel, "Id", "Name");

            ViewBag.RevenueTypeId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            var revenuesTypesModel = revenuesTypesServices.GetAll();
            ViewBag.RevenuesTypes = revenuesTypesModel;

            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            return View("Upsert", new Revenue());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Revenue revenue)
        {
            revenue.Id = Guid.NewGuid();
            var result = revenuesServices.Create(revenue, (Guid)TempData["UserId"]);
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
        public ActionResult Edit(Guid Id)
        {
            var revenue = revenuesServices.Get(Id);
            var revenuetype = revenuesTypesServices.GetAll().Where(x => x.Id == revenue.RevenueTypeId).FirstOrDefault();

            var studyPlaces = studyPlacesServices.GetAll();
            ViewBag.StudyPlaces = studyPlaces;

            var studyYears = studyYearsServices.GetAll();
            ViewBag.StudyYears = studyYears;

            var RevenueTypeParentIdModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
            ViewBag.RevenueTypeParentId = new SelectList(RevenueTypeParentIdModel, "Id", "Name", revenuetype.ParentId);

            var RevenueTypesModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == revenuetype.ParentId).ToList();
            ViewBag.RevenueTypeId = new SelectList(RevenueTypesModel, "Id", "Name", revenue.RevenueTypeId);

            var classes = classesServices.GetAll().Where(x => x.StudyPlaceId == revenue.StudyPlaceId).ToList();
            ViewBag.ClassId = new SelectList(classes,"Id","Name", revenue.ClassId);

            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            ViewBag.RevenueDate= revenue.RevenueDate.Value.ToString("yyyy-MM-dd");
            return View("Upsert", revenue);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Revenue revenue)
        {

            var result = revenuesServices.Edit(revenue, (Guid)TempData["UserId"]);
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
        public ActionResult Delete(Guid Id)
        {
            var result = revenuesServices.Delete(Id, (Guid)TempData["UserId"]);
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
        [Authorized(ScreenId = "67")]

        public ActionResult Reports()
        {
            var revenuesTypesParentModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
            ViewBag.RevenueTypeParentId = new SelectList(revenuesTypesParentModel, "Id", "Name");

            ViewBag.RevenueTypeId = new SelectList("");
            ViewBag.StudyPlaceId = new SelectList(studyPlacesServices.GetAll(), "Id", "Name");
            ViewBag.ClassId = new SelectList("");
            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "67")]

        public ActionResult Reports(string date, string date2, Guid? RevenueTypeParentId = null, Guid? RevenueTypeId = null, Guid? StudyPlaceId = null, Guid? ClassId = null)
        {
            var total = 0.0;
            var model = revenuesServices.GetAll();
            //if (AllYear)
            //{
            //    var year = DateTime.Now.Date.Year;
            //    model = model.Where(x => x.Date.Year == year).ToList();
            //}
            //if (Month != "")
            //{
            //    var mon = DateTime.Parse(Month).ToString("MM-yyyy");
            //    model = model.Where(x => x.Date.ToString("MM-yyyy") == mon).ToList();
            //}
            if (date != "" && date != null)
            {
                var mon = DateTime.Parse(date).Year;
                var mon2 = DateTime.Parse(date).Month;
                var mon3 = DateTime.Parse(date).Day;

                model = model.Where(x => x.Date.Year >= mon && x.Date.Month >= mon2 && x.Date.Day >= mon3).ToList();
            }
            if (date2 != "" && date2 != null)
            {
                var mon = DateTime.Parse(date2).Year;
                var mon2 = DateTime.Parse(date2).Month;
                var mon3 = DateTime.Parse(date2).Day;

                model = model.Where(x => x.Date.Year <= mon && x.Date.Month <= mon2 && x.Date.Day <= mon3).ToList();
            }
            if (StudyPlaceId != null)
            {
                model = model.Where(x => x.StudyPlaceId == StudyPlaceId).ToList();

                var StudyPlaceModel = studyPlacesServices.GetAll();
                ViewBag.StudyPlaceId = new SelectList(StudyPlaceModel, "Id", "Name", StudyPlaceId);
            }
            else
            {
                var StudyPlaceModel = studyPlacesServices.GetAll();
                ViewBag.StudyPlaceId = new SelectList(StudyPlaceModel, "Id", "Name");
            }
            if (ClassId != null)
            {
                model = model.Where(x => x.ClassId == ClassId).ToList();

                var classes = classesServices.GetAll().Where(x => x.StudyPlaceId == StudyPlaceId).ToList();
                ViewBag.ClassId = new SelectList(classes, "Id", "Name", StudyPlaceId);
            }
            else if (StudyPlaceId != null)
            {
                var classes = classesServices.GetAll().Where(x => x.StudyPlaceId == StudyPlaceId).ToList();
                ViewBag.ClassId = new SelectList(classes, "Id", "Name", StudyPlaceId);
            }
            else
            {
                ViewBag.ClassId = new SelectList("");
            }
            if (RevenueTypeParentId != null)
            {
                model = model.Where(x => x.RevenueTypeParentId == RevenueTypeParentId).ToList();
                var revenuesTypesParentModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
                ViewBag.RevenueTypeParentId = new SelectList(revenuesTypesParentModel, "Id", "Name", RevenueTypeParentId);
            }
            else
            {
                var revenuesTypesParentModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
                ViewBag.RevenueTypeParentId = new SelectList(revenuesTypesParentModel, "Id", "Name");
            }
            if (RevenueTypeId != null)
            {
                model = model.Where(x => x.RevenueTypeId == RevenueTypeId).ToList();
                var revenuesTypesModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == RevenueTypeParentId).ToList();
                ViewBag.RevenueTypeId = new SelectList(revenuesTypesModel, "Id", "Name", RevenueTypeId);
            }
            else if (RevenueTypeParentId != null)
            {
                var revenuesTypesModel = revenuesTypesServices.GetAll().Where(x => x.ParentId == RevenueTypeParentId).ToList();
                ViewBag.RevenueTypeId = new SelectList(revenuesTypesModel, "Id", "Name");
            }
            else
            {
                ViewBag.RevenueTypeId = new SelectList("");
            }
            foreach (var item in model)
            {
                total += float.Parse(item.Value);
            }
            ViewBag.Total = total;
            ViewBag.Revenues = model;
            return View();
           
        }
        [Authorized(ScreenId = "67")]
        public ActionResult getRevenuesTypesReport(Guid Id)
        {
            var model = revenuesTypesServices.GetAll().Where(x => x.ParentId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getRevenuesTypes(Guid Id)
        {
            var model = revenuesTypesServices.GetAll().Where(x => x.ParentId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getClasses(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.StudyPlaceId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "67")]

        public ActionResult getClassesReport(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.StudyPlaceId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
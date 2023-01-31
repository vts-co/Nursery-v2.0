using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
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
    [Authorized(Role = Role.SystemAdmin)]
    public class RevenuesController : Controller
    {
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();

        RevenuesServices revenuesServices = new RevenuesServices();
        RevenuesTypesServices revenuesTypesServices = new RevenuesTypesServices();
        EmployeesServices employeesServices = new EmployeesServices();

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
                revenue.Id = Guid.Empty;

                var studyPlaces = studyPlacesServices.GetAll();
                ViewBag.StudyPlaces = studyPlaces;

                var studyYears = studyYearsServices.GetAll();
                ViewBag.StudyYears = studyYears;

                var revenuesTypesModel = revenuesTypesServices.GetAll();
                ViewBag.RevenuesTypes = revenuesTypesModel;

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", revenue);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var studyPlaces = studyPlacesServices.GetAll();
            ViewBag.StudyPlaces = studyPlaces;

            var studyYears = studyYearsServices.GetAll();
            ViewBag.StudyYears = studyYears;

            var revenuesTypesModel = revenuesTypesServices.GetAll();
            ViewBag.RevenuesTypes = revenuesTypesModel;

            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var revenue = revenuesServices.Get(Id);
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
                var studyPlaces = studyPlacesServices.GetAll();
                ViewBag.StudyPlaces = studyPlaces;

                var studyYears = studyYearsServices.GetAll();
                ViewBag.StudyYears = studyYears;

                var revenuesTypesModel = revenuesTypesServices.GetAll();
                ViewBag.RevenuesTypes = revenuesTypesModel;

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                ViewBag.RevenueDate = revenue.RevenueDate.Value.ToString("yyyy-MM-dd");

                TempData["warning"] = result.Message;
                return View("Upsert", revenue);
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
        public ActionResult Reports()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Reports(string Month, bool AllYear)
        {
            if (AllYear)
            {
                var year = DateTime.Now.Date.Year;
                var model = revenuesServices.GetAll().Where(x => x.Date.Year == year);
                ViewBag.Revenues = model;
                return View();
            }
            else
            {
                var mon = DateTime.Parse(Month).ToString("MM-yyyy");
                var model = revenuesServices.GetAll().Where(x => x.Date.ToString("MM-yyyy") == mon);
                ViewBag.Revenues = model;
                return View();
            }
        }
    }
}
using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Employees;
using NurseryProject.Services.Expenses;
using NurseryProject.Services.ExpensesTypes;
using NurseryProject.Services.StudyPlaces;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "40")]

    public class ExpensesController : Controller
    {
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();

        ExpensesServices expensesServices = new ExpensesServices();
        ExpensesTypesServices expensesTypesServices = new ExpensesTypesServices();
        EmployeesServices employeesServices = new EmployeesServices();
        ClassesServices classesServices = new ClassesServices();


        // GET: Destricts
        public ActionResult Index()
        {
            var model = expensesServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyPlaces = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudyPlaces = studyPlaces;

            var studyYears = studyYearsServices.GetAll();
            ViewBag.StudyYears = studyYears;

            var expensesTypesParentModel = expensesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
            ViewBag.ExpenseTypeParentId = new SelectList(expensesTypesParentModel, "Id", "Name");

            ViewBag.ExpenseTypeId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            var employeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.Employees = employeesModel;

            return View("Upsert", new Expens());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Expens expens)
        {
            expens.Id = Guid.NewGuid();
            var result = expensesServices.Create(expens, (Guid)TempData["UserId"]);
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
            var expenses = expensesServices.Get(Id);
            var expensetype = expensesTypesServices.GetAll().Where(x => x.Id == expenses.ExpenseTypeId).FirstOrDefault();


            var studyPlaces = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudyPlaces = studyPlaces;

            var studyYears = studyYearsServices.GetAll();
            ViewBag.StudyYears = studyYears;

            var expensesTypesParentModel = expensesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
            ViewBag.ExpenseTypeParentId = new SelectList(expensesTypesParentModel, "Id", "Name", expensetype.ParentId);

            var expensesTypesModel = expensesTypesServices.GetAll().Where(x => x.ParentId == expensetype.ParentId).ToList();
            ViewBag.ExpenseTypeId = new SelectList(expensesTypesModel, "Id", "Name", expenses.ExpenseTypeId);

            var classes = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.StudyPlaceId == expenses.StudyPlaceId).ToList();
            ViewBag.ClassId = new SelectList(classes, "Id", "Name", expenses.ClassId);

            var employeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.Employees = employeesModel;

            ViewBag.ExpenseDate = expenses.ExpenseDate.Value.ToString("yyyy-MM-dd");

            return View("Upsert", expenses);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Expens expens)
        {

            var result = expensesServices.Edit(expens, (Guid)TempData["UserId"]);
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
            var result = expensesServices.Delete(Id, (Guid)TempData["UserId"]);
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

        [Authorized(ScreenId = "66")]
        public ActionResult Reports()
        {
            var expensesTypesParentModel = expensesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
            ViewBag.ExpenseTypeParentId = new SelectList(expensesTypesParentModel, "Id", "Name");
            ViewBag.StudyPlaceId = new SelectList(studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
            ViewBag.ClassId = new SelectList("");

            ViewBag.ExpenseTypeId = new SelectList("");
            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "66")]
        public ActionResult Reports(string date, string date2, Guid? ExpenseTypeParentId=null, Guid? ExpenseTypeId=null,Guid? StudyPlaceId=null,Guid? ClassId=null)
        {
            var total = 0.0;
            var model = expensesServices.GetAll();
            //if (AllYear)
            //{
            //    var year = DateTime.Now.Date.Year;
            //    model=model.Where(x => x.Date.Year == year).ToList();
            //}
            //if (Month != "")
            //{
            //    var mon = DateTime.Parse(Month).ToString("MM-yyyy");
            //    model=model.Where(x => x.Date.ToString("MM-yyyy") == mon).ToList();
            //}
            if (date != "" && date != null)
            {
                var mon = DateTime.Parse(DateTime.Parse(date).ToString("yyyy-MM-dd")).Year;
                var mon2 = DateTime.Parse(DateTime.Parse(date).ToString("yyyy-MM-dd")).Month;
                var mon3 = DateTime.Parse(DateTime.Parse(date).ToString("yyyy-MM-dd")).Day;

                model = model.Where(x => x.Date.Year >= mon && x.Date.Month >= mon2 && x.Date.Day >= mon3).ToList();
            }
            if (date2 != "" && date2 != null)
            {
                var mon = DateTime.Parse(DateTime.Parse(date2).ToString("yyyy-MM-dd")).Year;
                var mon2 = DateTime.Parse(DateTime.Parse(date2).ToString("yyyy-MM-dd")).Month;
                var mon3 = DateTime.Parse(DateTime.Parse(date2).ToString("yyyy-MM-dd")).Day;

                model = model.Where(x => x.Date.Year <= mon && x.Date.Month <= mon2 && x.Date.Day <= mon3).ToList();
            }

            if (StudyPlaceId != null)
            {
                model = model.Where(x => x.StudyPlaceId == StudyPlaceId).ToList();

                var StudyPlaceModel = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudyPlaceId = new SelectList(StudyPlaceModel, "Id", "Name", StudyPlaceId);
            }
            else
            {
                var StudyPlaceModel = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudyPlaceId = new SelectList(StudyPlaceModel, "Id", "Name");
            }
            if (ClassId != null)
            {
                model = model.Where(x => x.ClassId == ClassId).ToList();

                var classes = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x=>x.StudyPlaceId== StudyPlaceId).ToList();
                ViewBag.ClassId = new SelectList(classes, "Id", "Name", StudyPlaceId);
            }
            else if (StudyPlaceId != null)
            {
                var classes = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.StudyPlaceId == StudyPlaceId).ToList();
                ViewBag.ClassId = new SelectList(classes, "Id", "Name", StudyPlaceId);
            }
            else
            {
                ViewBag.ClassId = new SelectList("");
            }

            if (ExpenseTypeParentId != null)
            {
                model=model.Where(x => x.ExpenseTypePatentId== ExpenseTypeParentId).ToList();
                var expensesTypesParentModel = expensesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
                ViewBag.ExpenseTypeParentId = new SelectList(expensesTypesParentModel, "Id", "Name", ExpenseTypeParentId);
            }
            else
            {
                var expensesTypesParentModel = expensesTypesServices.GetAll().Where(x => x.ParentId == Guid.Empty).ToList();
                ViewBag.ExpenseTypeParentId = new SelectList(expensesTypesParentModel, "Id", "Name");
            }

            if (ExpenseTypeId != null)
            {
                model=model.Where(x => x.ExpenseTypeId == ExpenseTypeId).ToList();

                var expensesTypesModel = expensesTypesServices.GetAll().Where(x => x.ParentId == ExpenseTypeParentId).ToList();
                ViewBag.ExpenseTypeId = new SelectList(expensesTypesModel, "Id", "Name", ExpenseTypeId);
            }
            else if (ExpenseTypeParentId != null)
            {
                var expensesTypesModel = expensesTypesServices.GetAll().Where(x => x.ParentId == ExpenseTypeParentId).ToList();
                ViewBag.ExpenseTypeId = new SelectList(expensesTypesModel, "Id", "Name");
            }
            else
            {
                ViewBag.ExpenseTypeId = new SelectList("");
            }
            foreach (var item in model)
            {
                total += float.Parse(item.Value);
            }
            ViewBag.Total = total;
            ViewBag.Expenses = model;
            return View();
        }
        [Authorized(ScreenId = "66")]
        public ActionResult getExpensesTypesReport(Guid Id)
        {
            var model = expensesTypesServices.GetAll().Where(x => x.ParentId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getExpensesTypes(Guid Id)
        {
            var model = expensesTypesServices.GetAll().Where(x => x.ParentId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getClasses(Guid Id)
        {
            var model = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.StudyPlaceId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "66")]

        public ActionResult getClassesReport(Guid Id)
        {
            var model = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.StudyPlaceId == Id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
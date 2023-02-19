using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesIncreases;
using NurseryProject.Services.IncreasesTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "48")]

    public class EmployeesIncreasesController : Controller
    {
        EmployeesServices employeesServices = new EmployeesServices();
        IncreasesTypesServices increasesTypesServices = new IncreasesTypesServices();
        EmployeesIncreasesServices employeesIncreasesServices = new EmployeesIncreasesServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeesIncreasesServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var IncreasesTypesModel = increasesTypesServices.GetAll();
            ViewBag.IncreasesTypes = IncreasesTypesModel;

            return View("Upsert", new EmployeesIncreas());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeesIncreas employeesIncreas)
        {
            employeesIncreas.Id = Guid.NewGuid();
            var result = employeesIncreasesServices.Create(employeesIncreas, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesIncreas.Id = Guid.Empty;

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                var IncreasesTypesModel = increasesTypesServices.GetAll();
                ViewBag.IncreasesTypes = IncreasesTypesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesIncreas);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var IncreasesTypesModel = increasesTypesServices.GetAll();
            ViewBag.IncreasesTypes = IncreasesTypesModel;

            var employeesVacation = employeesIncreasesServices.Get(Id);
            ViewBag.Date = employeesVacation.IncreaseDate.Value.ToString("yyyy-MM-dd");

            return View("Upsert", employeesVacation);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeesIncreas employeesIncreas)
        {

            var result = employeesIncreasesServices.Edit(employeesIncreas, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Date = employeesIncreas.IncreaseDate.Value.ToString("yyyy-MM-dd");

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                var IncreasesTypesModel = increasesTypesServices.GetAll();
                ViewBag.IncreasesTypes = IncreasesTypesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesIncreas);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeesIncreasesServices.Delete(Id, (Guid)TempData["UserId"]);
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
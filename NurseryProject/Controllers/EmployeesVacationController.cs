using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesVacation;
using NurseryProject.Services.VacationsTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeesVacationController : Controller
    {
        EmployeesServices employeesServices = new EmployeesServices();
        VacationsTypesServices vacationsTypesServices = new VacationsTypesServices();
        EmployeesVacationServices employeesVacationServices = new EmployeesVacationServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeesVacationServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var vacationsTypesModel = vacationsTypesServices.GetAll();
            ViewBag.VacationsTypes = vacationsTypesModel;

            return View("Upsert", new EmployeesVacation());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeesVacation employeesVacation)
        {
            employeesVacation.Id = Guid.NewGuid();
            var result = employeesVacationServices.Create(employeesVacation, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesVacation.Id = Guid.Empty;

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                var vacationsTypesModel = vacationsTypesServices.GetAll();
                ViewBag.VacationsTypes = vacationsTypesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesVacation);
            }
        }
        public ActionResult Edit(Guid Id)
        {

            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var vacationsTypesModel = vacationsTypesServices.GetAll();
            ViewBag.VacationsTypes = vacationsTypesModel;

            var employeesVacation = employeesVacationServices.Get(Id);
            ViewBag.DateFrom = employeesVacation.DateFrom.Value.ToString("yyyy-MM-dd");
            ViewBag.DateTo = employeesVacation.DateTo.Value.ToString("yyyy-MM-dd");

            return View("Upsert", employeesVacation);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeesVacation employeesVacation)
        {

            var result = employeesVacationServices.Edit(employeesVacation, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.DateFrom = employeesVacation.DateFrom.Value.ToString("yyyy-MM-dd");
                ViewBag.DateTo = employeesVacation.DateTo.Value.ToString("yyyy-MM-dd");

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                var vacationsTypesModel = vacationsTypesServices.GetAll();
                ViewBag.VacationsTypes = vacationsTypesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesVacation);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeesVacationServices.Delete(Id, (Guid)TempData["UserId"]);
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
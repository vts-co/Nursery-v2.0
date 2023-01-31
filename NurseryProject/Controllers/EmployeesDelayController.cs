using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesDelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeesDelayController : Controller
    {
        EmployeesServices employeesServices = new EmployeesServices();
        EmployeesDelayServices employeesDelayServices = new EmployeesDelayServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeesDelayServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            return View("Upsert", new EmployeesDelay());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeesDelay employeesDelay)
        {
            employeesDelay.Id = Guid.NewGuid();
            var result = employeesDelayServices.Create(employeesDelay, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesDelay.Id = Guid.Empty;

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDelay);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var employeesVacation = employeesDelayServices.Get(Id);
            ViewBag.Date = employeesVacation.DelayDate.Value.ToString("yyyy-MM-dd");

            return View("Upsert", employeesVacation);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeesDelay employeesDelay)
        {

            var result = employeesDelayServices.Edit(employeesDelay, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Date = employeesDelay.DelayDate.Value.ToString("yyyy-MM-dd");

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDelay);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeesDelayServices.Delete(Id, (Guid)TempData["UserId"]);
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
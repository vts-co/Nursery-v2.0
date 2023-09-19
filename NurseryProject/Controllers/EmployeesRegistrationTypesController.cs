using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.EmployeesRegistrationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "73")]

    public class EmployeesRegistrationTypesController : Controller
    {
        // GET: RegistrationTypes
        // GET: Departments
        RegistrationTypesServices registrationTypesServices = new RegistrationTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = registrationTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new EmployeeRegistrationType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeeRegistrationType registrationType)
        {
            registrationType.Id = Guid.NewGuid();
            var result = registrationTypesServices.Create(registrationType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                registrationType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", registrationType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var registrationType = registrationTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", registrationType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeeRegistrationType registrationType)
        {

            var result = registrationTypesServices.Edit(registrationType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", registrationType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = registrationTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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
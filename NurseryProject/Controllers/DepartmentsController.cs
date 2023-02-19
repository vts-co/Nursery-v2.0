using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Departments;
using NurseryProject.Services.RegistrationTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "7")]

    public class DepartmentsController : Controller
    {
        // GET: Departments
        DepartmentsServices departmentsServices = new DepartmentsServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = departmentsServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new Department());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Department department)
        {
            department.Id = Guid.NewGuid();
            var result = departmentsServices.Create(department, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                department.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", department);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var department = departmentsServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", department);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Department department)
        {

            var result = departmentsServices.Edit(department, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", department);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = departmentsServices.Delete(Id, (Guid)TempData["UserId"]);
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
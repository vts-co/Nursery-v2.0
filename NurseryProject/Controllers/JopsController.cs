using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Jops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class JopsController : Controller
    {
        // GET: Jops
        DepartmentsServices departmentsServices = new DepartmentsServices();
        JopsServices jopsServices = new JopsServices();

        // GET: Destricts
        public ActionResult Index()
        {

            var model = jopsServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var cityModel = departmentsServices.GetAll();
            ViewBag.Departments = cityModel;

            return View("Upsert", new Jop());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Jop jop)
        {
            jop.Id = Guid.NewGuid();
            var result = jopsServices.Create(jop, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                jop.Id = Guid.Empty;

                var cityModel = departmentsServices.GetAll();
                ViewBag.Departments = cityModel;

                TempData["warning"] = result.Message;
                return View("Upsert", jop);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var cityModel = departmentsServices.GetAll();
            ViewBag.Departments = cityModel;

            var jop = jopsServices.Get(Id);
            return View("Upsert", jop);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Jop jop)
        {

            var result = jopsServices.Edit(jop, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var cityModel = departmentsServices.GetAll();
                ViewBag.Departments = cityModel;


                TempData["warning"] = result.Message;
                return View("Upsert", jop);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = jopsServices.Delete(Id, (Guid)TempData["UserId"]);
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
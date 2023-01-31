using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.DiscountsTypes;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesDiscounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeesDiscountsController : Controller
    {
        EmployeesServices employeesServices = new EmployeesServices();
        DiscountsTypesServices discountsTypesServices = new DiscountsTypesServices();
        EmployeesDiscountsServices employeesDiscountsServices = new EmployeesDiscountsServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeesDiscountsServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var DiscountsTypesModel = discountsTypesServices.GetAll();
            ViewBag.DiscountsTypes = DiscountsTypesModel;

            return View("Upsert", new EmployeesDiscount());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeesDiscount employeesDiscount)
        {
            employeesDiscount.Id = Guid.NewGuid();
            var result = employeesDiscountsServices.Create(employeesDiscount, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesDiscount.Id = Guid.Empty;

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                var DiscountsTypesModel = discountsTypesServices.GetAll();
                ViewBag.DiscountsTypes = DiscountsTypesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDiscount);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeesModel = employeesServices.GetAll();
            ViewBag.Employees = employeesModel;

            var DiscountsTypesModel = discountsTypesServices.GetAll();
            ViewBag.DiscountsTypes = DiscountsTypesModel;

            var employeesDiscounts = employeesDiscountsServices.Get(Id);
            ViewBag.Date = employeesDiscounts.DiscountDate.Value.ToString("yyyy-MM-dd");

            return View("Upsert", employeesDiscounts);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeesDiscount employeesDiscount)
        {

            var result = employeesDiscountsServices.Edit(employeesDiscount, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Date = employeesDiscount.DiscountDate.Value.ToString("yyyy-MM-dd");

                var employeesModel = employeesServices.GetAll();
                ViewBag.Employees = employeesModel;

                var DiscountsTypesModel = discountsTypesServices.GetAll();
                ViewBag.DiscountsTypes = DiscountsTypesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesDiscount);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeesDiscountsServices.Delete(Id, (Guid)TempData["UserId"]);
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
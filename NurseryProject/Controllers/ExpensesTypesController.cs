using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.ExpensesTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class ExpensesTypesController : Controller
    {
        ExpensesTypesServices expensesTypesServices = new ExpensesTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = expensesTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new ExpensesType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(ExpensesType expensesType)
        {
            expensesType.Id = Guid.NewGuid();
            var result = expensesTypesServices.Create(expensesType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                expensesType.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", expensesType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var expensesType = expensesTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", expensesType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(ExpensesType expensesType)
        {

            var result = expensesTypesServices.Edit(expensesType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", expensesType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = expensesTypesServices.Delete(Id, (Guid)TempData["UserId"]);
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
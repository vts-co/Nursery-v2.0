using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesTransferAllowance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeesTransferAllowanceController : Controller
    {
        EmployeesServices employeesServices = new EmployeesServices();
        EmployeesTransferAllowanceServices employeesTransferAllowanceServices = new EmployeesTransferAllowanceServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeesTransferAllowanceServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var employeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.Employees = employeesModel;

            return View("Upsert", new EmployeesTransferAllowance());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeesTransferAllowance employeesTransferAllowance)
        {
            employeesTransferAllowance.Id = Guid.NewGuid();
            var result = employeesTransferAllowanceServices.Create(employeesTransferAllowance, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesTransferAllowance.Id = Guid.Empty;

                var employeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.Employees = employeesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesTransferAllowance);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.Employees = employeesModel;

            var employeesTransferAllowance = employeesTransferAllowanceServices.Get(Id);
            ViewBag.Date = employeesTransferAllowance.TransferDate.Value.ToString("yyyy-MM-dd");

            return View("Upsert", employeesTransferAllowance);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeesTransferAllowance employeesTransferAllowance)
        {

            var result = employeesTransferAllowanceServices.Edit(employeesTransferAllowance, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Date = employeesTransferAllowance.TransferDate.Value.ToString("yyyy-MM-dd");

                var employeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.Employees = employeesModel;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesTransferAllowance);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeesTransferAllowanceServices.Delete(Id, (Guid)TempData["UserId"]);
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
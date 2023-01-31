using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesWorkShifts;
using NurseryProject.Services.Jops;
using NurseryProject.Services.StudyPlaces;
using NurseryProject.Services.StudyYears;
using NurseryProject.Services.WorkShifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeesWorkShiftsController : Controller
    {
        JopsServices jopsServices = new JopsServices();
        DepartmentsServices departmentsServices = new DepartmentsServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        EmployeesServices employeesServices = new EmployeesServices();
        WorkShiftsServices workShiftsServices = new WorkShiftsServices();
        EmployeesWorkShiftsServices employeesWorkShiftsServices = new EmployeesWorkShiftsServices();
        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeesWorkShiftsServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var Years = studyYearsServices.GetAll();
            ViewBag.StudyYears = Years;

            var Departments = departmentsServices.GetAll();
            ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name");

            ViewBag.EmployeeId = new SelectList("");

            var WorkShiftsModel = workShiftsServices.GetAll();
            ViewBag.WorkShifts = WorkShiftsModel;

            return View("Upsert", new EmployeesWorkShift());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeesWorkShift employeesWorkShift,List<Guid> emp)
        {
            var result = employeesWorkShiftsServices.Create(employeesWorkShift, emp, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesWorkShift.Id = Guid.Empty;
                var emp1 = employeesServices.Get(employeesWorkShift.EmployeeId.Value).JopId;
                var jop = jopsServices.Get(emp1.Value).DepartmentId;
                var employeesModel = employeesServices.GetAll();
                ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", employeesWorkShift.EmployeeId);

                var Departments = departmentsServices.GetAll();
                ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", jop);

                var WorkShiftsModel = workShiftsServices.GetAll();
                ViewBag.WorkShifts = WorkShiftsModel;

                var Years = studyYearsServices.GetAll();
                ViewBag.StudyYears = Years;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesWorkShift);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeesWorkShift = employeesWorkShiftsServices.Get(Id);

            var emp = employeesServices.Get(employeesWorkShift.EmployeeId.Value).JopId;
            var jop = jopsServices.Get(emp.Value).DepartmentId;
            var employeesModel = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", employeesWorkShift.EmployeeId);

            var Departments = departmentsServices.GetAll();
            ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", jop);

            var Years = studyYearsServices.GetAll();
            ViewBag.StudyYears = Years;

            var WorkShiftsModel = workShiftsServices.GetAll();
            ViewBag.WorkShifts = WorkShiftsModel;

            return View("Upsert", employeesWorkShift);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeesWorkShift employeesWorkShift)
        {

            var result = employeesWorkShiftsServices.Edit(employeesWorkShift, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var emp = employeesServices.Get(employeesWorkShift.EmployeeId.Value).JopId;
                var jop = jopsServices.Get(emp.Value).DepartmentId;
                var employeesModel = employeesServices.GetAll();
                ViewBag.EmployeeId = new SelectList(employeesModel, "Id", "Name", employeesWorkShift.EmployeeId);

                var Departments = departmentsServices.GetAll();
                ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", jop);

                var WorkShiftsModel = workShiftsServices.GetAll();
                ViewBag.WorkShifts = WorkShiftsModel;
                var Years = studyYearsServices.GetAll();
                ViewBag.StudyYears = Years;

                TempData["warning"] = result.Message;
                return View("Upsert", employeesWorkShift);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeesWorkShiftsServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult getEmployees(Guid DepartmentId)
        {
            var model = employeesServices.GetAll().Where(x => x.DepartmentId == DepartmentId).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
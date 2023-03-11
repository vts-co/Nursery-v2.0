using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesAttendance;
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
    [Authorized(ScreenId = "36")]

    public class EmployeesAttendanceController : Controller
    {
        JopsServices jopsServices = new JopsServices();
        DepartmentsServices departmentsServices = new DepartmentsServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        EmployeesServices employeesServices = new EmployeesServices();
        WorkShiftsServices workShiftsServices = new WorkShiftsServices();
        EmployeesWorkShiftsServices employeesWorkShiftsServices = new EmployeesWorkShiftsServices();
        EmployeesAttendanceServices employeesAttendanceServices = new EmployeesAttendanceServices();
        // GET: Destricts
        public ActionResult Index()
        {
            var model = employeesAttendanceServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var Years = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(Years, "Id", "Name");

            //var Departments = departmentsServices.GetAll();
            //ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name");

            var workshift = workShiftsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.WorkShiftId = new SelectList(workshift, "Id", "Name");

            return View("Upsert", new EmployeesAttendance());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeesAttendance employeesAttendance, List<Attend1> emp)
        {
            var result = employeesAttendanceServices.Create(employeesAttendance, emp, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employeesAttendance.Id = Guid.Empty;

                var Years = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(Years, "Id", "Name");

                //var Departments = departmentsServices.GetAll();
                //ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name");

                var workshift = workShiftsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.WorkShiftId = new SelectList(workshift, "Id", "Name");
                TempData["warning"] = result.Message;
                return View("Upsert", new EmployeesAttendance());
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employeesWorkShift = employeesAttendanceServices.Get(Id);
            //var employee = employeesServices.Get(employeesWorkShift.EmployeeId.Value);
            //var jop = jopsServices.Get(employee.JopId.Value);

            var Years = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(Years, "Id", "Name", employeesWorkShift.StudyYearId);

            //var Departments = departmentsServices.GetAll();
            //ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", jop.DepartmentId);

            var workshift = workShiftsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.WorkShiftId = new SelectList(workshift, "Id", "Name", employeesWorkShift.WorkShiftId);
            ViewBag.Date = employeesWorkShift.Date;
            ViewBag.readon = "readonly";
            return View("Upsert", new EmployeesAttendance() { Id= employeesWorkShift.Id});
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeesAttendance employeesAttendance, List<Attend1> emp)
        {
            var result = employeesAttendanceServices.Edit(employeesAttendance, emp, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var employeesWorkShift = employeesWorkShiftsServices.Get(employeesAttendance.EmployeeWorkShiftId.Value);
                var employee = employeesServices.Get(employeesWorkShift.EmployeeId.Value);
                var jop = jopsServices.Get(employee.JopId.Value);

                var Years = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(Years, "Id", "Name", employeesWorkShift.StudyYearId);

                //var Departments = departmentsServices.GetAll();
                //ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", jop.DepartmentId);
                ViewBag.readon = "readonly";

                var workshift = workShiftsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.WorkShiftId = new SelectList(workshift, "Id", "Name", employeesWorkShift.WorkShiftId.Value);
                return View("Upsert", new EmployeesAttendance() { Id = employeesWorkShift.Id });
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeesAttendanceServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult getEmployees(Guid Id)
        {
            var model = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).Select(x => new { x.Id, x.Name, x.Code }).FirstOrDefault();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getEmployeesWorkShifts(Guid StudyYearId,Guid WorkShiftId)
        {
            var model = employeesWorkShiftsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x =>x.WorkShiftId == WorkShiftId && x.StudyYearId == StudyYearId).Select(x=>new {x.Id,x.Code,x.EmployeeId,x.EmployeeName }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getEmployeesWorkShiftsBysearch(Guid StudyYearId, Guid WorkShiftId,string search)
        {
            var model = employeesWorkShiftsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.WorkShiftId == WorkShiftId && x.StudyYearId == StudyYearId).Select(x => new { x.Id, x.Code, x.EmployeeId, x.EmployeeName }).ToList();
            if (search != null)
            {
                model = model.Where(x => x.Code.Contains(search) || x.EmployeeName.Contains(search)).Select(x => new { x.Id, x.Code, x.EmployeeId, x.EmployeeName }).ToList();
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getEmployeesAttendance(Guid Id, string Date)
        {
            var model = employeesAttendanceServices.GetByEmployeeWorkShift().Where(x => x.EmployeeWorkShiftId == Id && x.Date == Date&&x.IsAttend==true).Select(x => new { x.Id,x.Code, x.EmployeeId, x.EmployeeName,x.EmployeeWorkShiftId }).ToList().Count();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
    public class Attend1
    {
        public Guid Id { get; set; }
        public string Att { get; set; }

    }
}
using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.BuildingSupervisors;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.StudyPlaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "34")]

    public class BuildingSupervisorsController : Controller
    {
        DepartmentsServices departmentsServices = new DepartmentsServices();

        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        EmployeesServices employeesServices = new EmployeesServices();

        BuildingSupervisorsServices buildingSupervisorsServices = new BuildingSupervisorsServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = buildingSupervisorsServices.GetAll((Guid)TempData["UserId"],(Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }

        public ActionResult Create()
        {
            var PlacesModel = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudyPlaces = PlacesModel;

            var departments = departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name");

            var EmployeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeId = new SelectList("");

            return View("Upsert", new BuildingSupervisor());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(BuildingSupervisor buildingSupervisor)
        {
            buildingSupervisor.Id = Guid.NewGuid();
            var result = buildingSupervisorsServices.Create(buildingSupervisor, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                buildingSupervisor.Id = Guid.Empty;

                var employee = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x=>x.Id== buildingSupervisor.EmployeeId.Value).FirstOrDefault();

                var departments = departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.DepartmentId = new SelectList(departments, "Id", "Name", employee.DepartmentId);
                
                var PlacesModel = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudyPlaces = PlacesModel;

                var EmployeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeId = new SelectList(EmployeesModel,"Id","Name", buildingSupervisor.EmployeeId);

                TempData["warning"] = result.Message;
                return View("Upsert", buildingSupervisor);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var destrict = buildingSupervisorsServices.Get(Id);

            var PlacesModel = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudyPlaces = PlacesModel;

            var EmployeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.EmployeeId = new SelectList(EmployeesModel, "Id", "Name", destrict.EmployeeId);

            var employee = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == destrict.EmployeeId.Value).FirstOrDefault();

            var departments = departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name", employee.DepartmentId);

            return View("Upsert", destrict);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(BuildingSupervisor buildingSupervisor)
        {

            var result = buildingSupervisorsServices.Edit(buildingSupervisor, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var employee = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == buildingSupervisor.EmployeeId.Value).FirstOrDefault();

                var departments = departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.DepartmentId = new SelectList(departments, "Id", "Name", employee.DepartmentId);

                var PlacesModel = studyPlacesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudyPlaces = PlacesModel;

                var EmployeesModel = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.EmployeeId = new SelectList(EmployeesModel, "Id", "Name", buildingSupervisor.EmployeeId);


                TempData["warning"] = result.Message;
                return View("Upsert", buildingSupervisor);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = buildingSupervisorsServices.Delete(Id, (Guid)TempData["UserId"]);
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
            var model = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.DepartmentId == DepartmentId).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}
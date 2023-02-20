using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.ClassesLeaders;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.StudyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "35")]

    public class ClassesLeadersController : Controller
    {
        DepartmentsServices departmentsServices = new DepartmentsServices();
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        EmployeesServices employeesServices = new EmployeesServices();
        ClassesLeadersServices classesLeadersServices = new ClassesLeadersServices();
        // GET: Destricts
        public ActionResult Index()
        {
            var model = classesLeadersServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var departments = departmentsServices.GetAll();
            ViewBag.DepartmentId = new SelectList(departments, "Id", "Name");

            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            var Employees = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList("");


            return View("Upsert", new ClassesLeader());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(ClassesLeader Class)
        {
            Class.Id = Guid.NewGuid();
            var result = classesLeadersServices.Create(Class, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                Class.Id = Guid.Empty;

                var studyTypes = studyTypesServices.GetAll();

                var class2 = classesServices.GetAll().Where(x => x.Id == Class.ClassId).FirstOrDefault();
                var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

                var Employees = employeesServices.GetAll();
                ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name", Class.EmployeeId);

                var Departments = departmentsServices.GetAll();
                ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", Class.Employee.Jop.DepartmentId);

                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var class1 = classesLeadersServices.Get(Id);
            var studyTypes = studyTypesServices.GetAll();

            var class2 = classesServices.GetAll().Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);


            var Employees = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name", class1.EmployeeId);

            var Departments = departmentsServices.GetAll();
            ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", class1.Employee.Jop.DepartmentId);

            return View("Upsert", class1);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(ClassesLeader Class)
        {

            var result = classesLeadersServices.Edit(Class, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var studyTypes = studyTypesServices.GetAll();

                var class2 = classesServices.GetAll().Where(x => x.Id == Class.ClassId).FirstOrDefault();
                var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

                var Employees = employeesServices.GetAll();
                ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name", Class.EmployeeId);

                var Departments = departmentsServices.GetAll();
                ViewBag.DepartmentId = new SelectList(Departments, "Id", "Name", Class.Employee.Jop.DepartmentId);


                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = classesLeadersServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult getLevels(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getClasses(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
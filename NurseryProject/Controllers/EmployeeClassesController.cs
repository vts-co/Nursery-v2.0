using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.EmployeeClasses;
using NurseryProject.Services.Employees;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using NurseryProject.Services.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeeClassesController : Controller
    {
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        SubjectsServices subjectsServices = new SubjectsServices();
        EmployeesServices employeesServices = new EmployeesServices();
        EmployeeClassesServices employeeClassesServices = new EmployeeClassesServices();
        // GET: Destricts
        public ActionResult Index()
        {

            var model = employeeClassesServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");
            ViewBag.SubjectId = new SelectList("");

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            var Employees = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name");


            return View("Upsert", new EmployeeClass());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(EmployeeClass Class)
        {
            Class.Id = Guid.NewGuid();
            var result = employeeClassesServices.Create(Class, (Guid)TempData["UserId"]);
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
                ViewBag.SubjectId = new SelectList(subjectsServices.GetAll().Where(x => x.LevelId == level.Id).ToList(), "Id", "Name", Class.SubjectId);

                var StudyYear = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", Class.StudyYearId);

                var Employees = employeesServices.GetAll();
                ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name", Class.EmployeeId);

                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var class1 = employeeClassesServices.Get(Id);
            var studyTypes = studyTypesServices.GetAll();

            var class2 = classesServices.GetAll().Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);
            ViewBag.SubjectId = new SelectList(subjectsServices.GetAll().Where(x => x.LevelId == level.Id).ToList(), "Id", "Name", class1.SubjectId);

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", class1.StudyYearId);

            var Employees = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name", class1.EmployeeId);

            return View("Upsert", class1);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(EmployeeClass Class)
        {

            var result = employeeClassesServices.Edit(Class, (Guid)TempData["UserId"]);
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
                ViewBag.SubjectId = new SelectList(subjectsServices.GetAll().Where(x => x.LevelId == level.Id).ToList(), "Id", "Name", Class.SubjectId);

                var StudyYear = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", Class.StudyYearId);

                var Employees = employeesServices.GetAll();
                ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name", Class.EmployeeId);

                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = employeeClassesServices.Delete(Id, (Guid)TempData["UserId"]);
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

        public ActionResult Reports()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");
            ViewBag.SubjectId = new SelectList("");

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            var Employees = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name");

            return View(new EmployeeClass());
        }
        [HttpPost]
        public ActionResult Reports(EmployeeClass employeeClass)
        {

            var class2 = classesServices.GetAll().Where(x => x.Id == employeeClass.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();
            var studyType = studyTypesServices.GetAll().Where(x => x.Id == level.StudyTypeId).FirstOrDefault();
            var studyTypes = studyTypesServices.GetAll();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x=>new { x.Id,Name= x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);
            ViewBag.SubjectId = new SelectList(subjectsServices.GetAll().Where(x => x.LevelId == level.Id).ToList(), "Id", "Name", employeeClass.SubjectId);

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", employeeClass.StudyYearId);

            var Employees = employeesServices.GetAll();
            ViewBag.EmployeeId = new SelectList(Employees, "Id", "Name", employeeClass.EmployeeId);


            var result = employeeClassesServices.GetAll();
            var model = result.Where(x => x.SubjectId == employeeClass.SubjectId && x.ClassId == employeeClass.ClassId && x.StudyYearId == employeeClass.StudyYearId && x.LevelId == level.Id && x.StudyTypeId == studyType.Id).ToList();
            ViewBag.Reports = model;
            return View(employeeClass);
        }
        public ActionResult getLevels(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getClasses(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name=x.Name+" ("+x.StudyPlaceName+")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getSubjects(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
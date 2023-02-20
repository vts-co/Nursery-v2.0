using NurseryProject.Authorization;
using NurseryProject.Dtos.EmployeeClasses;
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
    [Authorized(ScreenId = "33")]
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

        [Authorized(ScreenId = "60")]
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

            return View(new EmployeeClassesDto());
        }
        [HttpPost]
        [Authorized(ScreenId = "60")]

        public ActionResult Reports(EmployeeClassesDto employeeClass)
        {
            var result = employeeClassesServices.GetAll();
            var model = result;
            if (employeeClass.StudyYearId != null)
            {
                model = model.Where(x => x.StudyYearId == employeeClass.StudyYearId).ToList();
                var StudyYear = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", employeeClass.StudyYearId);
            }
            else
            {
                var StudyYear = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");
            }
            if (employeeClass.StudyTypeId != null)
            {
                model = model.Where(x => x.StudyTypeId == employeeClass.StudyTypeId).ToList();
                var studyTypes = studyTypesServices.GetAll();
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", employeeClass.StudyTypeId);
            }
            else
            {
                var studyTypes = studyTypesServices.GetAll();
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");
            }
            if (employeeClass.LevelId != null&& employeeClass.StudyTypeId != null)
            {
                model = model.Where(x => x.LevelId == employeeClass.LevelId).ToList();
                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == employeeClass.StudyTypeId).ToList(), "Id", "Name", employeeClass.LevelId);
            }
            else if (employeeClass.StudyTypeId != null)
            {
                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == employeeClass.StudyTypeId).ToList(), "Id", "Name");
            }
            else
            {
                ViewBag.LevelId = new SelectList("");
            }
            if (employeeClass.ClassId != null&& employeeClass.LevelId!=null)
            {
                model = model.Where(x => x.ClassId == employeeClass.ClassId).ToList();
                ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == employeeClass.LevelId).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", employeeClass.ClassId);
            }
            else if (employeeClass.LevelId != null)
            {
                ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == employeeClass.LevelId).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name");
            }
            else
            {
                ViewBag.ClassId = new SelectList("");
            }
            if (employeeClass.SubjectId != null&& employeeClass.LevelId != null)
            {
                model = model.Where(x => x.SubjectId == employeeClass.SubjectId).ToList();
                ViewBag.SubjectId = new SelectList(subjectsServices.GetAll().Where(x => x.LevelId == employeeClass.LevelId).ToList(), "Id", "Name", employeeClass.SubjectId);
            }
            else if (employeeClass.LevelId != null)
            {
                ViewBag.SubjectId = new SelectList(subjectsServices.GetAll().Where(x => x.LevelId == employeeClass.LevelId).ToList(), "Id", "Name");
            }
            else
            {
                ViewBag.SubjectId = new SelectList("");
            }
           
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
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getSubjects(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorized(ScreenId = "60")]
        public ActionResult getLevelsReport(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "60")]
        public ActionResult getClassesReport(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "60")]
        public ActionResult getSubjectsReport(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
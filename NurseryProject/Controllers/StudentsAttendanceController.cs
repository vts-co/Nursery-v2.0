using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsAttendance;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyClasses;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class StudentsAttendanceController : Controller
    {
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyClassesServices studyClassesServices = new StudyClassesServices();
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        StudentsServices studentsServices = new StudentsServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        StudentsAttendanceServices studentsAttendanceServices = new StudentsAttendanceServices();
        // GET: Destricts
        public ActionResult Index()
        {
            var model = studentsAttendanceServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");
            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name");
            ViewBag.StudyClassId = new SelectList("");

            var Students = studentsServices.GetAll();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");


            return View("Upsert", new StudentsAttendance());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudentsAttendance Class,List<Guid> IsAttend)
        {
            var result = studentsAttendanceServices.Create(Class, IsAttend, (Guid)TempData["UserId"]);
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

                ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name",Class.StudyClass.StudyYearId);
                ViewBag.StudyClassId = new SelectList(studyClassesServices.GetAll(),"Id","Name", Class.StudyClassId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

                var Students = studentsServices.GetAll();
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", Class.StudentId);
             

                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var class1 = studentsAttendanceServices.Get(Id);
            var studyTypes = studyTypesServices.GetAll();

            var class2 = classesServices.GetAll().Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

            var Students = studentsServices.GetAll();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name", class1.StudentId);

            return View("Upsert", class1);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudentsAttendance Class)
        {

            var result = studentsAttendanceServices.Edit(Class, (Guid)TempData["UserId"]);
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

                var Students = studentsServices.GetAll();
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", Class.StudentId);


                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studentsAttendanceServices.Delete(Id, (Guid)TempData["UserId"]);
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
            var students = studentsServices.GetAll();
            ViewBag.Students = students;
            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name");
            ViewBag.StudyClassId = new SelectList("");
            return View();
        }
        [HttpPost]
        public ActionResult Reports(string StudentId, string StudyYearId, string StudyClassId,string Month,string By)
        {
            var students = studentsServices.GetAll();
            ViewBag.Students = students;

            if (StudentId != null && StudyYearId != null && StudyClassId != null)
            {

                var studentId = Guid.Parse(StudentId);
                var studyYearId = Guid.Parse(StudyYearId);
                var studyClassId = Guid.Parse(StudyClassId);
                ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name", studyYearId);
                ViewBag.StudyClassId = new SelectList(studyClassesServices.GetAll(), "Id", "Name", studyClassId);

                var model = studentsAttendanceServices.GetAll().Where(x => x.StudyClassId == studyClassId && x.StudentId == studentId);
                var model2 = studentsAttendanceServices.GetAll().Where(x => x.StudyClassId == studyClassId && x.StudentId == studentId && x.IsAttend == false).Count();
                ViewBag.Count = model2;
                ViewBag.Attendance = model;
            }
            else if(StudentId != null && StudyYearId != null)
            {
                var studentId = Guid.Parse(StudentId);
                var studyYearId = Guid.Parse(StudyYearId);
                ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name", studyYearId);
                ViewBag.StudyClassId = new SelectList("");

                var model = studentsAttendanceServices.GetAll().Where(x => x.StudyYearId == studyYearId && x.StudentId == studentId);
                var model2 = studentsAttendanceServices.GetAll().Where(x => x.StudyYearId == studyYearId && x.StudentId == studentId && x.IsAttend == false).Count();
                ViewBag.Count = model2;
                ViewBag.Attendance = model;
                 
            }
            else if (StudentId != null && Month != null)
            {
                var studentId = Guid.Parse(StudentId);
                var month = DateTime.Parse(Month).ToString("yyyy-MM");
                ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name");
                ViewBag.StudyClassId = new SelectList("");
                var model = studentsAttendanceServices.GetAll().Where(x => x.Date.Contains(month)&& x.StudentId == studentId);
                var model2 = studentsAttendanceServices.GetAll().Where(x => x.Date.Contains(month)&& x.StudentId == studentId && x.IsAttend == false).Count();
                ViewBag.Count = model2;
                ViewBag.Attendance = model;
            }
            ViewBag.By = By;
           
            return View();

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
        public ActionResult getStudents(Guid ClassId)
        {
            var model = studentsClassServices.GetAll().Where(x => x.ClassId == ClassId).Select(x => new { x.Id, x.StudentId,x.Code,x.StudentName }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudyClass(Guid StudyYearId)
        {
            var model = studyClassesServices.GetAll().Where(x => x.StudyYearId == StudyYearId).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        
    }
}

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
    [Authorized(ScreenId = "29")]

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
            var model = studentsAttendanceServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
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

            var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");


            return View("Upsert", new StudentsAttendance());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudentsAttendance Class, List<Attend> IsAttend)
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
                var studyClass = studyClassesServices.Get(Class.StudyClassId.Value);

                var class2 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Class.ClassId).FirstOrDefault();
                var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

                ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name", studyClass.StudyYearId);
                ViewBag.StudyClassId = new SelectList(studyClassesServices.GetAll(), "Id", "Name", Class.StudyClassId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

                var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", Class.StudentId);


                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var class1 = studentsAttendanceServices.Get(Id);
            var studyTypes = studyTypesServices.GetAll();
            var studyClass = studyClassesServices.Get(class1.StudyClassId.Value);
            var class2 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);
            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name", studyClass.StudyYearId);
            ViewBag.StudyClassId = new SelectList(studyClassesServices.GetAll(), "Id", "Name", class1.StudyClassId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

            var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(Students, "Id", "Name", class1.StudentId);
            ViewBag.Date = class1.Date.Value.ToString("yyyy-MM-dd");
            ViewBag.readon = "readonly";

            return View("Upsert", class1);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudentsAttendance Class, List<Attend> IsAttend)
        {

            var result = studentsAttendanceServices.Edit(Class, IsAttend, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var studyTypes = studyTypesServices.GetAll();
                var studyClass = studyClassesServices.Get(Class.StudyClassId.Value);
                var class2 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Class.ClassId).FirstOrDefault();
                var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);
                ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name", studyClass.StudyYearId);
                ViewBag.StudyClassId = new SelectList(studyClassesServices.GetAll(), "Id", "Name", Class.StudyClassId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

                var Students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", Class.StudentId);
                ViewBag.Date = Class.Date.Value.ToString("yyyy-MM-dd");
                ViewBag.readon = "readonly";

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

        public ActionResult getLevels(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getClasses(Guid Id)
        {
            var model = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudents(Guid ClassId)
        {
            var model = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.ClassId == ClassId).Select(x => new { x.Id, x.StudentId, x.Code, x.StudentName }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudentsBysearch(Guid ClassId, string search)
        {
            var model = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.ClassId == ClassId).Select(x => new { x.Id, x.StudentId, x.Code, x.StudentName }).ToList();
            if (search != null)
            {
                model = model.Where(x => x.Code.Contains(search) || x.StudentName.Contains(search)).Select(x => new { x.Id, x.StudentId, x.Code, x.StudentName }).ToList();
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudentAttendans(string Date, Guid ClassId, Guid StudyYearId, Guid StudyClassId, Guid StudentId)
        {
            var date1 = DateTime.Parse(Date);
            var model = studentsAttendanceServices.Get(date1, ClassId, StudyYearId, StudyClassId, StudentId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudyClass(Guid StudyYearId)
        {
            var model = studyClassesServices.GetAll().Where(x => x.StudyYearId == StudyYearId).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorized(ScreenId = "62")]
        public ActionResult Reports()
        {
            var students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.Students = students;
            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name");
            ViewBag.StudyClassId = new SelectList("");
            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "62")]
        public ActionResult Reports(string StudentId, string StudyYearId, string StudyClassId, string Month, string Date, string Date2, string By)
        {
            var students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.Students = students;

            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name");
            ViewBag.StudyClassId = new SelectList("");

            var model = studentsAttendanceServices.GetAllByStudent((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            //var model2 = studentsAttendanceServices.GetAllNoAttendance();

            if (StudentId != "" && StudentId != null)
            {
                ViewBag.student = "1";
                var studentId = Guid.Parse(StudentId);
                model = model.Where(x => x.StudentId == studentId).ToList();
            }
            else
            {
                ViewBag.student = "2";
            }

            if (StudyYearId != null && StudyClassId != null && StudyYearId != "" && StudyClassId != "")
            {
                var studyYearId = Guid.Parse(StudyYearId);
                var studyClassId = Guid.Parse(StudyClassId);

                model = model.Where(x => x.StudyClassId == studyClassId).ToList();
            }
            else if (StudyYearId != "" && StudyYearId != null)
            {
                var studyYearId = Guid.Parse(StudyYearId);

                model = model.Where(x => x.StudyYearId == studyYearId).ToList();
            }
            else if (Month != null)
            {
                var month = DateTime.Parse(Month).ToString("yyyy-MM");
                foreach (var item in model)
                {
                    if (!item.Days.Any(y => y.Date.Contains(month)))
                    {
                        model.Remove(item);
                    }

                }
            }
            else if (Date != null && Date2 != null)
            {
                var date = DateTime.Parse(Date);
                var date2 = DateTime.Parse(Date2);

                foreach (var item in model)
                {
                    if (!item.Days.Any(y => DateTime.Parse(y.Date).AddDays(1) >= date && DateTime.Parse(y.Date) <= date2))
                    {
                        model.Remove(item);
                    }
                }
            }

            ViewBag.Count = model.Where(x => x.IsAttend == false).ToList().Count();
            ViewBag.Count2 = model.Where(x => x.IsAttend == true).ToList().Count();

            ViewBag.Attendance = model;
            ViewBag.By = By;
            return View();

        }
        [Authorized(ScreenId = "62")]
        public ActionResult getStudyClassReport(Guid StudyYearId)
        {
            var model = studyClassesServices.GetAll().Where(x => x.StudyYearId == StudyYearId).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
    public class Attend
    {
        public Guid Id { get; set; }
        public string Att { get; set; }

    }
}

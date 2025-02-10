using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Services.Classes;
using NurseryProject.Services.StudentExamDegrees;
using NurseryProject.Services.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "79")]

    public class ClassStudentsResultsController : Controller
    {
        LevelsServices levelsServices = new LevelsServices();
        StudentsServices studentsServices = new StudentsServices();
        StudentExamDegreesServices studentExamDegreesServices = new StudentExamDegreesServices();
        ClassesServices classesServices = new ClassesServices();
        // GET: ClassStudentsResults
        public ActionResult Index()
        {
            var students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(students, "Id", "Name");
            ViewBag.LevelId = new SelectList(levelsServices.GetAll(), "Id", "Name");
            ViewBag.ClassId = new SelectList("");
            return View();
        }
        [HttpPost]
        public ActionResult Index(string ClassId, string LevelId)
        {

            var students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll(), "Id", "Name");
            if (LevelId != "" && LevelId != null)
            {
                var LevelId1 = Guid.Parse(LevelId);

                ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == LevelId1).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name");

            }
            else
            {
                ViewBag.ClassId = new SelectList("");
            }
            var model = studentExamDegreesServices.GetClassStudentsResultAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"],Guid.Parse( ClassId));
            var exams= studentExamDegreesServices.GetAllClassExams((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"], Guid.Parse(ClassId));

            ViewBag.Exams = exams;
            ViewBag.Data = model;
            ViewBag.Total = exams.Sum(x => x.Degree);

            return View();
        }



        public ActionResult ClassYearResults()
        {
            ViewBag.LevelId = new SelectList(levelsServices.GetAll(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult ClassYearResults(string LevelId)
        {

            ViewBag.LevelId = new SelectList(levelsServices.GetAll(), "Id", "Name", LevelId);

            var model = studentExamDegreesServices.GetYearClasssResultAll(Guid.Parse( LevelId));

            return View(model);
        }


    }
}
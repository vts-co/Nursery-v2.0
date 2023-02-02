using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentExamDegrees;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Exams;
using NurseryProject.Services.StudentExamDegrees;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class StudentsExamDegreesController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        ExamsServices examsServices = new ExamsServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        StudentExamDegreesServices studentExamDegreesServices = new StudentExamDegreesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = studentExamDegreesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            var exams = examsServices.GetAll();
            ViewBag.ExamId = new SelectList("");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            return View("Upsert", new StudentExamDegreesDto());
        }
        [HttpPost]
        public ActionResult Create(StudentExamDegreesDto studentExamDegreesDto)
        {
            var model = studentExamDegreesServices.Create(studentExamDegreesDto, (Guid)TempData["UserId"]);
            if (model.IsSuccess)
            {
                TempData["success"] = model.Message;
                return RedirectToAction("Index");
            }
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", studentExamDegreesDto.StudyTypeId);

            var level = levelsServices.GetAll().Where(x => x.StudyTypeId == studentExamDegreesDto.StudyTypeId);
            ViewBag.LevelId = new SelectList(level, "Id", "Name", studentExamDegreesDto.LevelId);

            var class1 = classesServices.GetAll().Where(x => x.LevelId == studentExamDegreesDto.LevelId);
            ViewBag.ClassId = new SelectList(class1, "Id", "Name", studentExamDegreesDto.ClassId);

            var exams = examsServices.GetAll().Where(x => x.LevelId == studentExamDegreesDto.LevelId); ;
            ViewBag.ExamId = new SelectList(exams, "Id", "Name", studentExamDegreesDto.ExamId);

            return View("Upsert", new StudentExamDegreesDto());
        }

        public ActionResult Delete(Guid Id)
        {
            var result = studentExamDegreesServices.Delete(Id, (Guid)TempData["UserId"]);
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

        public ActionResult getClasses(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult get(Guid Id)
        { 
            var model = examsServices.GetAll().Where(x => x.Id == Id).Select(x => new { x.Id, Name = x.Name,x.IsOneQuestion}).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudents(Guid Id)
        {
            var model = studentsClassServices.GetAll().Where(x => x.ClassId == Id).Select(x => new { Id=x.StudentId,x.Code, Name = x.StudentName }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getExams(Guid Id)
        {
            var model = examsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.SubjectName + "/" + x.ExamTypeName + "/" + x.SubjectName, x.IsOneQuestion }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
using NurseryProject.Authorization;
using NurseryProject.Dtos.Exams;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Services.Exams;
using NurseryProject.Services.ExamsTypes;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "25")]

    public class ExamsController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        SubjectsServices subjectsServices = new SubjectsServices();
        ExamsTypesServices examsTypesServices = new ExamsTypesServices();
       
        ExamsServices examsServices = new ExamsServices();

        // GET: Destricts
        public ActionResult Index()
        {
            var model = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }

        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");
            var examsTypes = examsTypesServices.GetAll();
            ViewBag.ExamTypeId = new SelectList(examsTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.SubjectId = new SelectList("");
            ViewBag.check = "checked";
            return View("Upsert", new ExamsDto());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(ExamsDto examsDto)
        {
            examsDto.Id = Guid.NewGuid();
            
            var result = examsServices.Create(examsDto,(Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                examsDto.Id = Guid.Empty;

                var studyTypes = studyTypesServices.GetAll();
                
                var subject = subjectsServices.GetAll().Where(x => x.Id == examsDto.SubjectId).FirstOrDefault();
                var level = levelsServices.GetAll().Where(x => x.Id == subject.LevelId).FirstOrDefault();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

                var examsTypes = examsTypesServices.GetAll();
                ViewBag.ExamTypeId = new SelectList(examsTypes, "Id", "Name", examsDto.ExamTypeId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.SubjectId = new SelectList(subjectsServices.GetAll(), "Id", "Name", examsDto.SubjectId);

                TempData["warning"] = result.Message;
                return View("Upsert", examsDto);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var exam = examsServices.Get(Id);
            var studyTypes = studyTypesServices.GetAll();

            var subject = subjectsServices.GetAll().Where(x => x.Id == exam.SubjectId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == subject.LevelId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            var examsTypes = examsTypesServices.GetAll();
            ViewBag.ExamTypeId = new SelectList(examsTypes, "Id", "Name", exam.ExamTypeId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.SubjectId = new SelectList(subjectsServices.GetAll(), "Id", "Name", exam.SubjectId);


            return View("Upsert", exam);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(ExamsDto examsDto)
        {
           
            var result = examsServices.Edit(examsDto,(Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var studyTypes = studyTypesServices.GetAll();

                var subject = subjectsServices.GetAll().Where(x => x.Id == examsDto.SubjectId).FirstOrDefault();
                var level = levelsServices.GetAll().Where(x => x.Id == subject.LevelId).FirstOrDefault();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

                var examsTypes = examsTypesServices.GetAll();
                ViewBag.ExamTypeId = new SelectList(examsTypes, "Id", "Name", examsDto.ExamTypeId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.SubjectId = new SelectList(subjectsServices.GetAll(), "Id", "Name", examsDto.SubjectId);


                TempData["warning"] = result.Message;
                return View("Upsert", examsDto);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = examsServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult getSubjects(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getExamDegrees(Guid Id)
        {
            var model = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).Select(x => new { x.Id, count = x.MoreQuestion.Count(),total=x.TotalDegree,Degrees=x.MoreQuestion.ToList() }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        
    }
}
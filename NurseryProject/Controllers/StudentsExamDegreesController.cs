using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentExamDegrees;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Exams;
using NurseryProject.Services.StudentExamDegrees;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "30")]

    public class StudentsExamDegreesController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        ExamsServices examsServices = new ExamsServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        StudentExamDegreesServices studentExamDegreesServices = new StudentExamDegreesServices();
        StudentsServices studentsServices = new StudentsServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = studentExamDegreesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }
        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            var exams = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
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

            var class1 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == studentExamDegreesDto.LevelId);
            ViewBag.ClassId = new SelectList(class1, "Id", "Name", studentExamDegreesDto.ClassId);

            var exams = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == studentExamDegreesDto.LevelId); ;
            ViewBag.ExamId = new SelectList(exams, "Id", "Name", studentExamDegreesDto.ExamId);

            return View("Upsert", new StudentExamDegreesDto());
        }

        public ActionResult Edit(Guid Id)
        {
            var model = studentExamDegreesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).FirstOrDefault();

            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", model.StudyTypeId);

            var exams = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.ExamId = new SelectList(exams, "Id", "Name", model.ExamId);

            var level = levelsServices.GetAll();
            ViewBag.LevelId = new SelectList(level, "Id", "Name", model.LevelId);

            var class1 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.ClassId = new SelectList(class1, "Id", "Name", model.ClassId);

            return View("Upsert", model);
        }
        [HttpPost]
        public ActionResult Edit(StudentExamDegreesDto studentExamDegreesDto)
        {
            var model = studentExamDegreesServices.Edit(studentExamDegreesDto, (Guid)TempData["UserId"]);
            if (model.IsSuccess)
            {
                TempData["success"] = model.Message;
                return RedirectToAction("Index");
            }
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", studentExamDegreesDto.StudyTypeId);

            var level = levelsServices.GetAll().Where(x => x.StudyTypeId == studentExamDegreesDto.StudyTypeId);
            ViewBag.LevelId = new SelectList(level, "Id", "Name", studentExamDegreesDto.LevelId);

            var class1 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == studentExamDegreesDto.LevelId);
            ViewBag.ClassId = new SelectList(class1, "Id", "Name", studentExamDegreesDto.ClassId);

            var exams = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == studentExamDegreesDto.LevelId); ;
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
            var model = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult get(Guid Id)
        {
            var model = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).Select(x => new { x.Id, Name = x.Name, x.IsOneQuestion, x.TotalDegree, x.MoreQuestion }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudents(Guid Id)
        {
            var model = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.ClassId == Id).Select(x => new { Id = x.StudentId, x.Code, Name = x.StudentName }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getExams(Guid Id)
        {
            var model = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getLevels(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getExamClass(Guid Id)
        {
            var model = studentExamDegreesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).FirstOrDefault();
            var students = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.ClassId == model.ClassId).ToList();
            var count = model.Students.Count();

            for (int i = 0; i < students.Count(); i++)
            {
                var check = false;
                for (int j = 0; j < count; j++)
                {
                    if (model.Students[j].StudentId != students[i].StudentId)
                    {
                        continue;
                    }
                    else
                    {
                        check = true;
                        model.Students[j].Count = "1";
                        break;
                    }
                }
                if (!check)
                {
                    StudentExamDegreesDetailsDto studentExamDegreesDetailsDto = new StudentExamDegreesDetailsDto()
                    {
                        Code = students[i].Code,
                        StudentId = students[i].StudentId,
                        StudentName = students[i].StudentName,
                        TotalDegree = "",
                        Count = "0"
                    };
                    model.Students.Add(studentExamDegreesDetailsDto);
                }

            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getStudentExamDegrees(Guid Id, Guid StudentId, Guid ExamId)
        {
            List<StudentExamDegreesDetailsDto> studentExamDegreesDetailsDtos = new List<StudentExamDegreesDetailsDto>();
            var model = studentExamDegreesServices.StudentExamDegrees(Id, StudentId).ToList();
            var model1 = examsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == ExamId).FirstOrDefault();

            var item = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.StudentId == StudentId).FirstOrDefault();
            List<StudentExamDegreesDetailsDto> studentExamDegreesDetailsDto2 = new List<StudentExamDegreesDetailsDto>();

            if (model.Count() == 0)
            {

                foreach (var item3 in model1.MoreQuestion)
                {
                    StudentExamDegreesDetailsDto studentExamDegreesDetailsDto = new StudentExamDegreesDetailsDto();

                    studentExamDegreesDetailsDto.Code = item.Code;
                    studentExamDegreesDetailsDto.StudentId = item.StudentId;
                    studentExamDegreesDetailsDto.StudentName = item.StudentName;
                    studentExamDegreesDetailsDto.ExamDegreeId = item3.Id;
                    studentExamDegreesDetailsDto.TotalDegree = "";
                    studentExamDegreesDetailsDto2.Add(studentExamDegreesDetailsDto);
                }
                return Json(studentExamDegreesDetailsDto2, JsonRequestBehavior.AllowGet);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorized(ScreenId = "75")]
        public ActionResult Reports()
        {
            var students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(students, "Id", "Name");
            ViewBag.LevelId = new SelectList(levelsServices.GetAll(), "Id", "Name");
            ViewBag.ClassId = new SelectList("");
            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "75")]
        public ActionResult Reports(string StudentId, string ClassId, string LevelId, string Date, string Date2)
        {
            List<StudentExamDegreesDto> list = new List<StudentExamDegreesDto>();
            var students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(students, "Id", "Name");

            ViewBag.LevelId = new SelectList(levelsServices.GetAll(), "Id", "Name");

            var model = studentExamDegreesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            //var model2 = studentsAttendanceServices.GetAllNoAttendance();

            if (StudentId != "" && StudentId != null)
            {
                ViewBag.student = "1";
                var studentId = Guid.Parse(StudentId);
                foreach (var item in model)
                {
                    var data = item.Students.Where(x => x.StudentId == studentId).ToList();
                    item.Students = data;
                }
            }
            else
            {
                ViewBag.student = "2";

            }

            //if (StudyYearId != null && ClassId != null && StudyYearId != "" && ClassId != "")
            //{
            //    var studyYearId = Guid.Parse(StudyYearId);
            //    var ClassId1 = Guid.Parse(ClassId);

            //    model = model.Where(x => x.ClassId == ClassId1).ToList();
            //}
            if (LevelId != "" && LevelId != null)
            {
                var LevelId1 = Guid.Parse(LevelId);

                model = model.Where(x => x.LevelId == LevelId1).ToList();
                ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == LevelId1).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name");

            }
            else
            {
                ViewBag.ClassId = new SelectList("");
            }
            if (ClassId != "" && ClassId != null)
            {
                var ClassId1 = Guid.Parse(ClassId);

                model = model.Where(x => x.ClassId == ClassId1).ToList();
            }
            if (Date != null && Date2 != null)
            {
                var date = DateTime.Parse(Date);
                var date2 = DateTime.Parse(Date2);

                foreach (var item in model)
                {
                    if (DateTime.Parse(item.Date).AddDays(1) >= date && DateTime.Parse(item.Date) <= date2)
                    {
                        model.Remove(item);
                    }
                }
            }

            //ViewBag.Count = model.Where(x => x.IsAttend == false).ToList().Count();
            //ViewBag.Count2 = model.Where(x => x.IsAttend == true).ToList().Count();
            if (string.IsNullOrEmpty(StudentId))
                ViewBag.Degrees = list;
            else
            ViewBag.Degrees = model;
            //ViewBag.By = By;
            return View();

        }

        public ActionResult getClassesReport(Guid Id)
        {
            var model = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClassesTransfer;
using NurseryProject.Enums;
using NurseryProject.Services.Classes;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudentsClassesTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(Role = Role.SystemAdmin)]

    public class StudentsClassesTransferController : Controller
    {
        ClassesServices classesServices = new ClassesServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        StudentsClassesTransferServices studentsClassesTransferServices = new StudentsClassesTransferServices();
        public ActionResult Index(Guid StudentClassId)
        {
            ViewBag.StudentClass = StudentClassId;
            var model = studentsClassesTransferServices.GetAll().Where(x=>x.StudentClassId== StudentClassId).ToList();
            return View(model);
        }
        public ActionResult Create(Guid StudentClassId)
        {
            var strdentClass = studentsClassServices.Get(StudentClassId);
            var classto = classesServices.GetAll().Where(x => x.LevelId == strdentClass.LevelId && x.Id != strdentClass.ClassId).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            var treansfer = new StudentsClassesTransferDto() {
                Id=Guid.NewGuid(),
                StudentClassId= StudentClassId,
                Code = strdentClass.Code,
                StudentId = strdentClass.StudentId,
                StudentName= strdentClass.StudentName,
                StudyPlaceId= strdentClass.StudyPlaceId,
                StudyPlaceName= strdentClass.StudyPlaceName,
                StudyTypeId= strdentClass.StudyTypeId,
                StudyTypeName= strdentClass.StudyTypeName,
                StudyYearId= strdentClass.StudyYearId,
                StudyYearName= strdentClass.StudyYearName,
                LevelId= strdentClass.LevelId,
                LevelName = strdentClass.LevelName,
                ClassFromId = strdentClass.ClassId,
                ClassFromName= strdentClass.ClassName + " (" + strdentClass.StudyPlaceName + ")",
            };
            ViewBag.ClassesTo = classto;
            return View("Upsert", treansfer);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudentsClassesTransferDto Class)
        {
            var result = studentsClassesTransferServices.Create(Class, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                var strdentClass = studentsClassServices.UpateClassId(Class.StudentClassId,Class.ClassToId, (Guid)TempData["UserId"]);
                if(strdentClass.IsSuccess)
                {
                    TempData["success"] = result.Message;
                    return RedirectToAction("Index", new { StudentClassId = Class.StudentClassId });
                }
                TempData["warning"] = result.Message;
                return RedirectToAction("Index", new { StudentClassId = Class.StudentClassId });
            }
            else
            {
                TempData["warning"] = result.Message;
                return RedirectToAction("Index", new { StudentClassId = Class.StudentClassId });
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result1 = studentsClassesTransferServices.Get(Id);

            var result = studentsClassesTransferServices.Delete(Id, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index", new { StudentClassId = result1.StudentClassId });
            }
            else
            {
                TempData["warning"] = result.Message;
                return RedirectToAction("Index", new { StudentClassId = result1.StudentClassId });
            }
        }
    }
}
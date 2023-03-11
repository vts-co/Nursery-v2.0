using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Dtos.StudentsClassesTransfer;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudentsClassesTransfer;
using NurseryProject.Services.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "71")]

    public class StudentsClassesTransferController : Controller
    {
        ClassesServices classesServices = new ClassesServices();
        LevelsServices levelsServices = new LevelsServices();
        SubscriptionsServices subscriptionsServices = new SubscriptionsServices();

        StudentsServices studentsServices = new StudentsServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        StudentsClassesTransferServices studentsClassesTransferServices = new StudentsClassesTransferServices();
        public ActionResult Search()
        {
            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]),"Id","Name");
            return View(new List<StudentsClassDto>());
        }
        [HttpPost]
        public ActionResult Search(Guid StudentId)
        {
            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name", StudentId);
            var model = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.StudentId == StudentId &&x.IsCurrent==true).ToList();
            return View(model);
        }
        public ActionResult Index(Guid StudentClassId)
        {
            ViewBag.StudentClass = StudentClassId;
            var model = studentsClassesTransferServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x=>x.StudentClassId== StudentClassId).ToList();
            return View(model);
        }
        public ActionResult Create(Guid StudentClassId)
        {
            var strdentClass = studentsClassServices.Get(StudentClassId);
            var classto = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == strdentClass.LevelId && x.Id != strdentClass.ClassId).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
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
        public ActionResult getSubscriptions(Guid Id)
        {
            var model = subscriptionsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.IsAnother == true ? (x.Name + "/" + "أخري") : (x.SubscriptionTypeName + "/ المبلغ : " + x.Amount + " جنيه/ عدد الاقساط : " + x.InstallmentsNumber) });
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
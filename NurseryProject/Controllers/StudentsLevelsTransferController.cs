using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Levels.Services;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using NurseryProject.Services.Subscriptions;
using NurseryProject.Services.SubscriptionsMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "72")]

    public class StudentsLevelsTransferController : Controller
    {
        ClassesServices classesServices = new ClassesServices();
        LevelsServices levelsServices = new LevelsServices();
        SubscriptionsServices subscriptionsServices = new SubscriptionsServices();
        StudentsServices studentsServices = new StudentsServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        SubscriptionsMethodsServices subscriptionsMethodsServices = new SubscriptionsMethodsServices();


        // GET: StudentsLevelsTransfer

        public ActionResult Search()
        {
            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown(), "Id", "Name");
            return View(new List<StudentsClassDto>());
        }
        [HttpPost]
        public ActionResult Search(Guid StudentId)
        {
            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown(), "Id", "Name", StudentId);
            var model = studentsClassServices.GetAll().Where(x => x.StudentId == StudentId && x.IsCurrent == true).ToList();
            return View(model);
        }
        public ActionResult Index(Guid Id)
        {
            var subs = subscriptionsMethodsServices.GetAll().Where(x => x.StudentClassId == Id && x.IsDeleted == false && x.IsPaid == false).ToList().Count();
            if (subs > 0)
            {
                TempData["warning"] = "هذا الطالب لا يمكن نقله لعدم دفع جميع الاقساط";
                return RedirectToAction("Index");
            }
            var class1 = studentsClassServices.Get(Id);
            class1.IsAnother = false;
            var class2 = classesServices.GetAll().Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();
            var studyTypes = studyTypesServices.GetAll();
            var studyYear1 = studyYearsServices.GetAll().Where(x => x.Id == class1.StudyYearId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            var StudyYear = studyYearsServices.GetAll().Where(x => x.DisplayOrder > studyYear1.DisplayOrder).ToList();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.DisplayOrder > level.DisplayOrder && x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name");
            ViewBag.ClassId = new SelectList("");

            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");
            //ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name");

            ViewBag.SubscriptionId = new SelectList("");

            return View(class1);
        }
        [HttpPost]
        public ActionResult Index(StudentsClassDto Class)
        {
            var id = Class.Id;
            Class.Id = Guid.NewGuid();
            Class.IsCurrent = true;

            if (Class.IsAnother == true)
            {
                Class.SubscriptionId = null;
            }
            var result = studentsClassServices.Create(Class, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                var result2 = studentsClassServices.UpateCurrentId(id, (Guid)TempData["UserId"]);

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

                var StudyYear = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", Class.StudyYearId);

                var Students = studentsServices.GetAllDropDown();
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", Class.StudentId);

                var subscription = subscriptionsServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.IsAnother == true ? (x.Name + "/" + "أخري") : (x.SubscriptionTypeName + "/ المبلغ : " + x.Amount + "جنيه / عدد الاقساط : " + x.InstallmentsNumber) });
                ViewBag.SubscriptionId = new SelectList(subscription, "Id", "Name", Class.SubscriptionId);
                //ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", RegistrationTypeId);

                if (Class.JoiningDate != null)
                    ViewBag.JoiningDate = Class.JoiningDate;

                TempData["warning"] = result.Message;
                return View(Class);
            }
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
        public ActionResult getSubscriptions(Guid Id)
        {
            var model = subscriptionsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.IsAnother == true ? (x.Name + "/" + "أخري") : (x.SubscriptionTypeName + "/ المبلغ : " + x.Amount + " جنيه/ عدد الاقساط : " + x.InstallmentsNumber) });
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getSubscriptionsById(Guid Id)
        {
            var model = subscriptionsServices.GetAll().Where(x => x.Id == Id).Select(x => new { x.Id, x.Amount, x.InstallmentsNumber, x.IsAnother, Name = x.IsAnother == true ? (x.Name + "/" + "أخري") : (x.SubscriptionTypeName + "/ المبلغ : " + x.Amount + "جنيه / عدد الاقساط : " + x.InstallmentsNumber) }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
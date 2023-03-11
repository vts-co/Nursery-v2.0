using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Expenses;
using NurseryProject.Services.Revenues;
using NurseryProject.Services.Settings;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using NurseryProject.Services.Subjects;
using NurseryProject.Services.Subscriptions;
using NurseryProject.Services.SubscriptionsMethods;
using NurseryProject.Services.SubscriptionsTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "22")]

    public class SubscriptionsController : Controller
    {
        SettingsServices settingsServices = new SettingsServices();

        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        SubjectsServices subjectsServices = new SubjectsServices();
        SubscriptionsTypesServices subscriptionsTypesServices = new SubscriptionsTypesServices();
        SubscriptionsServices subscriptionsServices = new SubscriptionsServices();
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudentsServices studentsServices = new StudentsServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        SubscriptionsMethodsServices subscriptionsMethodsServices = new SubscriptionsMethodsServices();
        ClassesServices classesServices = new ClassesServices();
        // GET: Destricts
        public ActionResult Index()
        {
            var model = subscriptionsServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");

            var subscriptions = subscriptionsTypesServices.GetAll();
            ViewBag.SubscriptionTypes = subscriptions;

            return View("Upsert", new Subscription());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Subscription subscription)
        {
            subscription.Id = Guid.NewGuid();
            var result = subscriptionsServices.Create(subscription, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                subscription.Id = Guid.Empty;

                var level1 = levelsServices.Get(subscription.LevelId.Value);
                var StudyTypeId = level1.StudyTypeId;

                var studyTypes = studyTypesServices.GetAll();
                var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);
                ViewBag.LevelId = new SelectList(levels, "Id", "Name", subscription.LevelId.Value);

                var subscriptions = subscriptionsTypesServices.GetAll();
                ViewBag.SubscriptionTypes = subscriptions;

                TempData["warning"] = result.Message;
                return View("Upsert", subscription);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var subscription = subscriptionsServices.Get(Id);

            var subscriptions = subscriptionsTypesServices.GetAll();
            ViewBag.SubscriptionTypes = subscriptions;

            var level1 = levelsServices.Get(subscription.LevelId.Value);
            var StudyTypeId = level1.StudyTypeId;

            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);

            var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();
            ViewBag.LevelId = new SelectList(levels, "Id", "Name", subscription.LevelId.Value);

            return View("Upsert", subscription);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Subscription subscription)
        {
            var result = subscriptionsServices.Edit(subscription, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var level1 = levelsServices.Get(subscription.LevelId.Value);
                var StudyTypeId = level1.StudyTypeId;

                var studyTypes = studyTypesServices.GetAll();
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);

                var levels = levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList();
                ViewBag.LevelId = new SelectList(levels, "Id", "Name", subscription.LevelId);

                var subscriptions = subscriptionsTypesServices.GetAll();
                ViewBag.SubscriptionTypes = subscriptions;

                TempData["warning"] = result.Message;
                return View("Upsert", subscription);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = subscriptionsServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult getReportDesign()
        {

            var model = settingsServices.GetAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [Authorized(ScreenId = "28")]
        public ActionResult Collect(Guid Id)
        {
            var class1 = studentsClassServices.Get(Id);
            class1.SubscriptionName = class1.SubscriptionName + "/" + class1.Amount + "جنيه/" + class1.Number;

            var studyTypes = studyTypesServices.GetAll();

            var class2 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", class1.StudyYearId);

            var Students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(Students, "Id", "Name", class1.StudentId);

            var subscription = subscriptionsServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.IsAnother == true ? (x.Name + "/" + "أخري") : (x.SubscriptionTypeName + "/ المبلغ : " + x.Amount + "جنيه / عدد الاقساط : " + x.InstallmentsNumber) });
            ViewBag.SubscriptionId = new SelectList(subscription, "Id", "Name", class1.SubscriptionId);


            if (class1.JoiningDate != null)
                ViewBag.JoiningDate = class1.JoiningDate;
            foreach (var item in class1.SubscriptionMethod)
            {
                if (item.PaidDate == null)
                    item.PaidDate = item.Date;
            }
            return View(class1);
        }
        [Authorized(ScreenId = "28")]
        public ActionResult getSubscriptionsMethods(Guid Id)
        {
            var model = subscriptionsMethodsServices.GetAll().Where(x => x.StudentClassId == Id).OrderBy(x => x.OrderDisplay).Select(x => new { x.Id, x.Amount, Date = x.Date.Value.ToString("yyyy-MM-dd"), IsPaid = x.IsPaid, PaidDate = x.PaidDate != null ? x.PaidDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "", PaidAmount = x.PaidAmount == null ? "" : x.PaidAmount }).ToList();
            var model2 = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Id).FirstOrDefault();

            var IsAnother = false;
            if (model2 != null)
            {
                if (model2.IsAnother == true)
                {
                    IsAnother = true;
                }
            }

            var data = new { result = model, IsAnother = IsAnother, Num = model.Count(), Amoun = model.Sum(x => float.Parse(x.Amount)) };
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Collect(StudentsClassDto Class)
        {
            if (Class.IsAnother == true)
            {
                Class.SubscriptionId = null;
            }
            var result = studentsClassServices.Collect(Class, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {

                TempData["success"] = result.Message;
                return RedirectToAction("Index", "StudentsClass");
            }
            else
            {
                var studyTypes = studyTypesServices.GetAll();

                var class2 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == Class.ClassId).FirstOrDefault();
                var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
                ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

                var StudyYear = studyYearsServices.GetAll();
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", Class.StudyYearId);

                var Students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", Class.StudentId);

                var subscription = subscriptionsServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.IsAnother == true ? (x.Name + "/" + "أخري") : (x.SubscriptionTypeName + "/ المبلغ : " + x.Amount + "جنيه / عدد الاقساط : " + x.InstallmentsNumber) });
                ViewBag.SubscriptionId = new SelectList(subscription, "Id", "Name", Class.SubscriptionId);

                if (Class.JoiningDate != null)
                    ViewBag.JoiningDate = Class.JoiningDate;

                TempData["warning"] = result.Message;
                return View(Class);
            }
        }
       
    }
}
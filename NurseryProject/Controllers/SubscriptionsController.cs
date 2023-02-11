using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Expenses;
using NurseryProject.Services.Revenues;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
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
    [Authorized(Role = Role.SystemAdmin)]

    public class SubscriptionsController : Controller
    {
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
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
        public ActionResult Reports()
        {
            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult Reports(Guid? StudentId = null, Guid? StudyYearId = null)
        {
            var StudyYear = studyYearsServices.GetAll();
            var Students = studentsServices.GetAllDropDown();
            var result = studentsClassServices.GetAll();

            if (StudyYearId != null && StudyYearId != Guid.Empty)
            {
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", StudyYearId);
                result = result.Where(x => x.StudyYearId == StudyYearId).ToList();
            }
            else
            {
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");
            }
            if (StudentId != null && StudentId != Guid.Empty)
            {
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", StudentId);
                result = result.Where(x => x.StudentId == StudentId).ToList();

            }
            else
            {
                ViewBag.StudentId = new SelectList(Students, "Id", "Name");
            }

            ViewBag.Reports = result;
            return View();
        }
        public ActionResult LatecomersReports()
        {

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            return View();
        }
        [HttpPost]
        public ActionResult LatecomersReports(Guid? StudyYearId = null, Guid? StudyTypeId = null, Guid? LevelId = null, Guid? ClassId = null)
        {
            var result = studentsClassServices.GetAll().Where(x => x.SubscriptionMethod.Where(y => y.IsPaid == false && DateTime.Parse(y.Date).Date.AddDays(15) < DateTime.Now.Date).Count() > 0);

            var StudyYear = studyYearsServices.GetAll();
            var studyTypes = studyTypesServices.GetAll();
            var levels = levelsServices.GetAll();
            var classes = classesServices.GetAll();


            if (StudyYearId != null && StudyYearId != Guid.Empty)
            {
                result = result.Where(x => x.StudyYearId == StudyYearId);
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", StudyYearId);
            }
            else
            {
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", StudyYearId);
            }
            if (StudyTypeId != null && StudyTypeId != Guid.Empty)
            {
                result = result.Where(x => x.StudyTypeId == StudyTypeId);
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);
            }
            else
            {
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");
            }
            if (LevelId != null && LevelId != Guid.Empty)
            {
                result = result.Where(x => x.LevelId == LevelId);
                ViewBag.LevelId = new SelectList(levels.Where(x => x.StudyTypeId == StudyTypeId).ToList(), "Id", "Name", LevelId);
            }
            else
            {
                ViewBag.LevelId = new SelectList(levels.Where(x => x.StudyTypeId == StudyTypeId).ToList(), "Id", "Name");
            }
            if (ClassId != null && ClassId != Guid.Empty)
            {
                result = result.Where(x => x.ClassId == ClassId);
                ViewBag.ClassId = new SelectList(classes.Where(x => x.LevelId == LevelId).ToList(), "Id", "Name", ClassId);
            }
            else
            {
                ViewBag.ClassId = new SelectList(classes.Where(x => x.LevelId == LevelId).ToList(), "Id", "Name");
            }

            ViewBag.Reports = result;
            return View();
        }
        public ActionResult Collect(Guid Id)
        {
            var class1 = studentsClassServices.Get(Id);
            class1.SubscriptionName = class1.SubscriptionName + "/" + class1.Amount + "جنيه/" + class1.Number;

            var studyTypes = studyTypesServices.GetAll();

            var class2 = classesServices.GetAll().Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name", level.Id);
            ViewBag.ClassId = new SelectList(classesServices.GetAll().Where(x => x.LevelId == level.Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", class2.Id);

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", class1.StudyYearId);

            var Students = studentsServices.GetAllDropDown();
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
        public ActionResult getSubscriptionsMethods(Guid Id)
        {

            var model = subscriptionsMethodsServices.GetAll().Where(x => x.StudentClassId == Id).OrderBy(x => x.OrderDisplay).Select(x => new { x.Id, x.Amount, Date = x.Date.Value.ToString("yyyy-MM-dd"), IsPaid = x.IsPaid, PaidDate = x.PaidDate != null ? x.PaidDate.Value.ToString("yyyy-MM-dd") : "", PaidAmount = x.PaidAmount == null ? "" : x.PaidAmount }).ToList();
            var model2 = studentsClassServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();

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
        public ActionResult UpdateSubscriptionsMethods(Guid Id, string Amount, string Date,Guid StudentId,Guid StudyYearId ,string Id2 = null, float Sub = 0)
        {
            var model = subscriptionsMethodsServices.Update(Id, Amount, Date, Id2, Sub, (Guid)TempData["UserId"]);
            if (model)
            {
                
                return Json("Done", JsonRequestBehavior.AllowGet);
            }
            else
                return Json("Faild", JsonRequestBehavior.AllowGet);

        }
        public ActionResult CancelSubscriptionsMethodsPaid(Guid Id)
        {
            var model = subscriptionsMethodsServices.Cancel(Id, (Guid)TempData["UserId"]);
            if (model)
                return Json("Done", JsonRequestBehavior.AllowGet);
            else
                return Json("Faild", JsonRequestBehavior.AllowGet);

        }

    }
}
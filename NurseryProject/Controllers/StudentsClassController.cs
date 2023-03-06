using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.EmployeeClasses;
using NurseryProject.Services.Employees;
using NurseryProject.Services.RegistrationTypes;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using NurseryProject.Services.Subjects;
using NurseryProject.Services.Subscriptions;
using NurseryProject.Services.SubscriptionsMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "28")]

    public class StudentsClassController : Controller
    {
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        SubjectsServices subjectsServices = new SubjectsServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        StudentsServices studentsServices = new StudentsServices();
        EmployeesServices employeesServices = new EmployeesServices();
        EmployeeClassesServices employeeClassesServices = new EmployeeClassesServices();
        StudentsClassServices studentsClassServices = new StudentsClassServices();
        SubscriptionsServices subscriptionsServices = new SubscriptionsServices();
        SubscriptionsMethodsServices subscriptionsMethodsServices = new SubscriptionsMethodsServices();
        RegistrationTypesServices registrationTypes = new RegistrationTypesServices();
        // GET: Destricts
        public ActionResult Index()
        {
            //var studyTypes = studyTypesServices.GetAll();
            //ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            //var StudyYear = studyYearsServices.GetAll();
            //ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            //ViewBag.LevelId = new SelectList("");
            //ViewBag.ClassId = new SelectList("");
            //ViewBag.SubscriptionId = new SelectList("");

            var model = studentsClassServices.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name");

            ViewBag.SubscriptionId = new SelectList("");

            return View("Upsert", new StudentsClassDto());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(StudentsClassDto Class, Guid RegistrationTypeId)
        {
            Class.Id = Guid.NewGuid();
            if (Class.IsAnother == true)
            {
                Class.SubscriptionId = null;
            }
            Class.IsCurrent = true;
            var result = studentsClassServices.Create(Class, RegistrationTypeId, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
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
                ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", RegistrationTypeId);

                if (Class.JoiningDate != null)
                    ViewBag.JoiningDate = Class.JoiningDate;

                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var class1 = studentsClassServices.Get(Id);
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
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", Students.Where(x => x.Id == class1.StudentId).FirstOrDefault().RegistrationTypeId);


            if (class1.JoiningDate != null)
                ViewBag.JoiningDate = class1.JoiningDate;

            return View("Upsert", class1);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(StudentsClassDto Class, Guid RegistrationTypeId)
        {
            if (Class.IsAnother == true)
            {
                Class.SubscriptionId = null;
            }
            var result = studentsClassServices.Edit(Class, RegistrationTypeId, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
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
                ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", Students.Where(x => x.Id == Class.StudentId).FirstOrDefault().RegistrationTypeId);

                if (Class.JoiningDate != null)
                    ViewBag.JoiningDate = Class.JoiningDate;

                TempData["warning"] = result.Message;
                return View("Upsert", Class);
            }
        }

        public ActionResult StudentsClassPrevious(Guid StudentClassId)
        {
            var model = studentsClassServices.GetAll().Where(x => x.Id == StudentClassId).FirstOrDefault();
            return View(model);
        }
        public ActionResult StudentsLevelsTransfer(Guid Id)
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
        public ActionResult StudentsLevelsTransfer(StudentsClassDto Class)
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
        public ActionResult Delete(Guid Id)
        {
            var result = studentsClassServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult Search(Guid StudyYearId, Guid StudyTypeId, Guid LevelId, Guid ClassId, Guid SubscriptionId)
        {
            var result = studentsClassServices.GetAll();
            var model = result.Where(x => x.StudyYearId == StudyYearId).FirstOrDefault();
            return View("Upsert", new StudentsClassDto());
        }

        [Authorized(ScreenId = "61")]
        public ActionResult Reports()
        {
            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");

            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "61")]
        public ActionResult Reports(string StudentId, string StudyYearId)
        {
            var StudyYear = studyYearsServices.GetAll();
            var id = Guid.Parse(StudyYearId);
            var id2 = Guid.Parse(StudentId);

            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", Guid.Parse(StudyYearId));

            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name", Guid.Parse(StudyYearId));

            var result = studentsClassServices.GetAll();
            var model = result.Where(x => x.StudyYearId == id && x.StudentId == id2).FirstOrDefault();


            var result2 = employeeClassesServices.GetAll().Where(x => x.ClassId == model.ClassId && x.StudyYearId == id).ToList();
            ViewBag.Reports = result2;
            return View();
        }
        
        [Authorized(ScreenId = "58")]
        public ActionResult NoInstallmentsReports()
        {
            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "58")]
        public ActionResult NoInstallmentsReports(string StudyYearId)
        {
            var result = studentsClassServices.GetAll().Where(x => x.SubscriptionMethod.Count() == 0);
            var StudyYear = studyYearsServices.GetAll();

            if (StudyYearId != "")
            {
                var id = Guid.Parse(StudyYearId);

                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", id);
                result.Where(x => x.StudyYearId == id && x.SubscriptionMethod.Count == 0).ToList();
                ViewBag.Reports = result;

                return View();

            }

            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");
            ViewBag.Reports = result;
            return View();
        }

        [Authorized(ScreenId = "61")]
        public ActionResult getLevels(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "61")]
        public ActionResult getClasses(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "61")]
        public ActionResult getSubjects(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, x.Name }).ToList();
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
        public ActionResult getSubscriptionsMethods(Guid Id)
        {

            var model = subscriptionsMethodsServices.GetAll().Where(x => x.StudentClassId == Id).OrderBy(x => x.OrderDisplay).Select(x => new { x.Amount, Date = x.Date.Value.ToString("yyyy-MM-dd"), IsPaid = x.IsPaid, PaidDate = x.PaidDate != null ? x.PaidDate.Value.ToString("yyyy-MM-dd") : "", x.PaidAmount }).ToList();
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
    }
}
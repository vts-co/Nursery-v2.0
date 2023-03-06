using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Levels.Services;
using NurseryProject.Services.Classes;
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

namespace NurseryProject.Views
{
    [Authorized(ScreenId = "70")]

    public class StudentsSubscriptionsController : Controller
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
        // GET: StudentsSubscriptions
        
        public ActionResult Index()
        {
            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown(), "Id", "Name");
            return View(new List<StudentsClassDto>());
        }
        [HttpPost]
        public ActionResult Index(Guid StudentId)
        {
            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown(), "Id", "Name", StudentId);
            var model = studentsClassServices.GetAll().Where(x => x.StudentId == StudentId && x.IsCurrent == true).ToList();
            return View(model);
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


        public ActionResult UpdateSubscriptionsMethods(Guid Id, string Amount, string Date, Guid StudentId, Guid StudyYearId, string Id2 = null, float Sub = 0)
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
        public ActionResult getSubscriptionsMethods(Guid Id)
        {
            var model = subscriptionsMethodsServices.GetAll().Where(x => x.StudentClassId == Id).OrderBy(x => x.OrderDisplay).Select(x => new { x.Id, x.Amount, Date = x.Date.Value.ToString("yyyy-MM-dd"), IsPaid = x.IsPaid, PaidDate = x.PaidDate != null ? x.PaidDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : "", PaidAmount = x.PaidAmount == null ? "" : x.PaidAmount }).ToList();
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

        //تقرير إشتراك طالب

        [Authorized(ScreenId = "54")]
        public ActionResult Reports()
        {
            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");

            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "54")]
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
        [Authorized(ScreenId = "54")]
        public ActionResult getLevelsReport(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "54")]
        public ActionResult getClassesReport(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "54")]
        public ActionResult getSubjectsReport(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        //تقرير الاقساط المتأخرة

        [Authorized(ScreenId = "56")]
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
        [Authorized(ScreenId = "56")]
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
        [Authorized(ScreenId = "56")]
        public ActionResult getLevelsLatecomersReport(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "56")]
        public ActionResult getClassesLatecomersReport(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        //تقرير الطلاب المنتظمة والغير منتظمة

        [Authorized(ScreenId = "57")]
        public ActionResult StudentRegularReports()
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
        [Authorized(ScreenId = "57")]
        public ActionResult StudentRegularReports(Guid? StudyYearId = null, Guid? StudyTypeId = null, Guid? LevelId = null, Guid? ClassId = null)
        {
            var result = studentsClassServices.GetAll();

            var StudyYear = studyYearsServices.GetAll();
            var studyTypes = studyTypesServices.GetAll();
            var levels = levelsServices.GetAll();
            var classes = classesServices.GetAll();


            if (StudyYearId != null && StudyYearId != Guid.Empty)
            {
                result = result.Where(x => x.StudyYearId == StudyYearId).ToList();
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", StudyYearId);
            }
            else
            {
                ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", StudyYearId);
            }
            if (StudyTypeId != null && StudyTypeId != Guid.Empty)
            {
                result = result.Where(x => x.StudyTypeId == StudyTypeId).ToList();
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);
            }
            else
            {
                ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");
            }
            if (LevelId != null && LevelId != Guid.Empty)
            {
                result = result.Where(x => x.LevelId == LevelId).ToList();
                ViewBag.LevelId = new SelectList(levels.Where(x => x.StudyTypeId == StudyTypeId).ToList(), "Id", "Name", LevelId);
            }
            else
            {
                ViewBag.LevelId = new SelectList(levels.Where(x => x.StudyTypeId == StudyTypeId).ToList(), "Id", "Name");
            }
            if (ClassId != null && ClassId != Guid.Empty)
            {
                result = result.Where(x => x.ClassId == ClassId).ToList();
                ViewBag.ClassId = new SelectList(classes.Where(x => x.LevelId == LevelId).ToList(), "Id", "Name", ClassId);
            }
            else
            {
                ViewBag.ClassId = new SelectList(classes.Where(x => x.LevelId == LevelId).ToList(), "Id", "Name");
            }

            ViewBag.Reports = result;
            return View();
        }
        [Authorized(ScreenId = "57")]
        public ActionResult getLevelsStudentRegularReport(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "57")]
        public ActionResult getClassesStudentRegularReport(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        //تقرير التحصيل اليومي للطلاب

        [Authorized(ScreenId = "59")]
        public ActionResult DailyReports()
        {
            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");

            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "59")]
        public ActionResult DailyReports(string Date, string Date2, Guid? StudentId = null)
        {
            var StudyYear = studyYearsServices.GetAll();
            var Students = studentsServices.GetAllDropDown();
            var date = DateTime.Parse(Date).ToString("yyyy-MM-dd");
            var date2 = DateTime.Parse(Date2).ToString("yyyy-MM-dd");

            var result = studentsClassServices.GetDayMoney(date, date2);


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
        [Authorized(ScreenId = "59")]
        public ActionResult getLevelsDailyReport(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "59")]
        public ActionResult getClassesDailyReport(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "59")]
        public ActionResult getSubjectsDailyReport(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        //التحاقات الطالب السابقة

        [Authorized(ScreenId = "55")]
        public ActionResult PreviousReports()
        {
            var Students = studentsServices.GetAllDropDown();
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");

            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "55")]
        public ActionResult PreviousReports(Guid? StudentId = null, Guid? StudyYearId = null)
        {
            var StudyYear = studyYearsServices.GetAll();
            var Students = studentsServices.GetAllDropDown();
            var result = studentsClassServices.GetAllPrevious(StudentId.Value);


            if (StudentId != null && StudentId != Guid.Empty)
            {
                ViewBag.StudentId = new SelectList(Students, "Id", "Name", StudentId);
            }
            else
            {
                ViewBag.StudentId = new SelectList(Students, "Id", "Name");
            }

            ViewBag.Reports = result;
            return View();
        }
        [Authorized(ScreenId = "55")]
        public ActionResult getLevelsPreviousReport(Guid Id)
        {
            var model = levelsServices.GetAll().Where(x => x.StudyTypeId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "55")]
        public ActionResult getClassesPreviousReport(Guid Id)
        {
            var model = classesServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [Authorized(ScreenId = "55")]
        public ActionResult getSubjectsPreviousReport(Guid Id)
        {
            var model = subjectsServices.GetAll().Where(x => x.LevelId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }



}
using ExcelDataReader;
using NurseryProject.Authorization;
using NurseryProject.Dtos.StudentsClass;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Classes;
using NurseryProject.Services.RegistrationTypes;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudentsClass;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using NurseryProject.Services.Subscriptions;
using NurseryProject.Services.SubscriptionsMethods;
using NurseryProject.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        RegistrationTypesServices registrationTypes = new RegistrationTypesServices();
        GenerateRandomCode randomCode = new GenerateRandomCode();

        // GET: StudentsLevelsTransfer

        public ActionResult Search()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            var Students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name");

            ViewBag.SubscriptionId = new SelectList("");
            ViewBag.JoiningDate = DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
            return View(new List<StudentsClassDto>());
        }
        [HttpPost]
        public ActionResult Search(Guid StudentId)
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            var Students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            ViewBag.StudentId = new SelectList(Students, "Id", "Name");
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name");

            ViewBag.SubscriptionId = new SelectList("");
            ViewBag.JoiningDate = DateTime.Now.ToString("yyyy-MM-dd");


            ViewBag.StudentId = new SelectList(studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name", StudentId);
            var model = studentsClassServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.StudentId == StudentId && x.IsCurrent == true).ToList();
            return View(model);
        }
        public ActionResult Index(Guid Id)
        {
            var subs = subscriptionsMethodsServices.GetAll().Where(x => x.StudentClassId == Id && x.IsDeleted == false && x.IsPaid == false).ToList().Count();
            if (subs > 0)
            {
                TempData["warning"] = "هذا الطالب لا يمكن نقله لعدم دفع جميع الاقساط";
                return RedirectToAction("Search");
            }
            var class1 = studentsClassServices.Get(Id);
            class1.IsAnother = false;
            var class2 = classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == class1.ClassId).FirstOrDefault();
            var level = levelsServices.GetAll().Where(x => x.Id == class2.LevelId).FirstOrDefault();
            var studyTypes = studyTypesServices.GetAll();
            var studyYear1 = studyYearsServices.GetAll().Where(x => x.Id == class1.StudyYearId).FirstOrDefault();

            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", level.StudyTypeId);

            var StudyYear = studyYearsServices.GetAll().Where(x => x.DisplayOrder > studyYear1.DisplayOrder).ToList();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.DisplayOrder > level.DisplayOrder && x.StudyTypeId == level.StudyTypeId).ToList(), "Id", "Name");
            ViewBag.ClassId = new SelectList("");

            var Students = studentsServices.GetAllDropDown((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
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
                //ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", RegistrationTypeId);

                if (Class.JoiningDate != null)
                    ViewBag.JoiningDate = Class.JoiningDate;

                TempData["warning"] = result.Message;
                return View(Class);
            }
        }

        #region Import MCQ Question Excel

        public ActionResult ImportExcel(HttpPostedFileBase upload, StudentsClassDto Class)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        TempData["warning"] = "البيانات المدخلة غير صحيحة";
                        return RedirectToAction("Search");
                    }
                    int fieldcount = reader.FieldCount;
                    int rowcount = reader.RowCount;
                    DataTable model = new DataTable();
                    DataRow row;
                    DataTable dt_ = new DataTable();
                    try
                    {
                        dt_ = reader.AsDataSet().Tables[0];
                        for (int i = 0; i < dt_.Columns.Count; i++)
                        {
                            var ss = dt_.Rows[0][i].ToString();
                            model.Columns.Add(dt_.Rows[0][i].ToString());
                        }
                        int rowcounter = 0;
                        for (int row_ = 1; row_ < rowcount; row_++)
                        {
                            row = model.NewRow();
                            for (int col = 0; col < fieldcount; col++)
                            {
                                row[col] = dt_.Rows[row_][col].ToString();
                                var sss = dt_.Rows[row_][col].ToString();

                                rowcounter++;
                            }
                            model.Rows.Add(row);

                        }

                    }
                    catch (Exception ex)
                    {
                        TempData["warning"] = "البيانات المدخلة غير صحيحة";
                        return RedirectToAction("Search");
                    }

                    reader.Close();
                    reader.Dispose();
                    for (int i = 0; i < model.Rows.Count; i++)
                    {
                        Student student = new Student();

                        var student1 = studentsServices.GetByCode(model.Rows[i][0].ToString(), model.Rows[i][1].ToString());
                        if (student1 == null)
                        {
                            student.Id = Guid.NewGuid();
                            if (model.Rows[i][0].ToString() == "" || model.Rows[i][0].ToString() == null)
                                student.Code = randomCode.GenerateStudentCodeRandom();
                            else if (studentsServices.CodeExist(model.Rows[i][0].ToString()))
                                student.Code = randomCode.GenerateStudentCodeRandom();
                            else
                                student.Code = model.Rows[i][0].ToString();

                            student.Name = model.Rows[i][1].ToString();
                            student.Phone = model.Rows[i][2].ToString();
                            student.Address = model.Rows[i][3].ToString();
                            student.BirthDate = model.Rows[i][4].ToString();
                            if (model.Rows[i][5].ToString() == "ذكر")
                                student.GenderId = (int)Gender.ذكر;
                            else if (model.Rows[i][5].ToString() == "انثي")
                                student.GenderId = (int)Gender.انثي;

                            student.MotherName = model.Rows[i][6].ToString();
                            student.JoiningDate = model.Rows[i][7].ToString();
                            student.Notes = model.Rows[i][8].ToString();
                            studentsServices.Create(student, (Guid)TempData["UserId"]);

                        }
                        else
                        {
                            student = student1;
                        }
                        var studentClass = studentsClassServices.GetByStudentCurent(student.Id);

                        var subs = subscriptionsMethodsServices.GetAll().Where(x => x.StudentClassId == studentClass.Id && x.IsDeleted == false && x.IsPaid == false).ToList().Count();
                        if(subs==0)
                        {
                            Class.Id = Guid.NewGuid();
                            Class.IsCurrent = true;
                            Class.StudentId = student.Id;

                            var result = studentsClassServices.Create(Class, (Guid)TempData["UserId"]);
                            if (result.IsSuccess)
                            {
                                var result2 = studentsClassServices.UpateCurrentId(studentClass.Id, (Guid)TempData["UserId"]);
                            }
                        }
                        
                    }
                    TempData["success"] = "تم حفظ البيانات بنجاح";
                    return RedirectToAction("Search");
                }
                else
                {
                    TempData["warning"] = "البيانات المدخلة غير صحيحة";
                    return RedirectToAction("Search");
                }
            }
            return View();
        }

        #endregion
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
        public ActionResult getSubscriptionsById(Guid Id)
        {
            var model = subscriptionsServices.GetAll().Where(x => x.Id == Id).Select(x => new { x.Id, x.Amount, x.InstallmentsNumber, x.IsAnother, Name = x.IsAnother == true ? (x.Name + "/" + "أخري") : (x.SubscriptionTypeName + "/ المبلغ : " + x.Amount + "جنيه / عدد الاقساط : " + x.InstallmentsNumber) }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
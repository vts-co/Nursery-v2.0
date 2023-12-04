using ExcelDataReader;
using NurseryProject.Authorization;
using NurseryProject.Dtos.Students;
using NurseryProject.Enums;
using NurseryProject.Levels.Services;
using NurseryProject.Models;
using NurseryProject.Services.Cities;
using NurseryProject.Services.Classes;
using NurseryProject.Services.Destricts;
using NurseryProject.Services.RegistrationTypes;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudyTypes;
using NurseryProject.Services.StudyYears;
using NurseryProject.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "27")]

    public class StudentsController : Controller
    {
        StudyYearsServices studyYearsServices = new StudyYearsServices();
        StudentsServices studentsServices = new StudentsServices();
        GenerateRandomCode randomCode = new GenerateRandomCode();
        RegistrationTypesServices registrationTypes = new RegistrationTypesServices();
        CitiesServices citiesServices = new CitiesServices();
        DestrictsServices destrictsServices = new DestrictsServices();
        StudyTypesServices studyTypesServices = new StudyTypesServices();
        LevelsServices levelsServices = new LevelsServices();
        ClassesServices classesServices = new ClassesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name");

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name");

            ViewBag.LevelId = new SelectList("");
            ViewBag.ClassId = new SelectList("");

            var model = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Index(Guid? StudyTypeId, Guid? StudyYearId, Guid? LevelId, Guid? ClassId)
        {
            var studyTypes = studyTypesServices.GetAll();
            ViewBag.StudyTypeId = new SelectList(studyTypes, "Id", "Name", StudyTypeId);

            var StudyYear = studyYearsServices.GetAll();
            ViewBag.StudyYearId = new SelectList(StudyYear, "Id", "Name", StudyYearId);

            var model = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);

            if (StudyYearId != null && StudyYearId != Guid.Empty)
            {
                model = model.Where(x => x.StudyYearId == StudyYearId).ToList();
            }
            if (StudyTypeId != null && StudyTypeId != Guid.Empty)
            {
                ViewBag.LevelId = new SelectList(levelsServices.GetAll().Where(x => x.StudyTypeId == StudyTypeId).ToList(), "Id", "Name", LevelId);
                if (LevelId != null && LevelId != Guid.Empty)
                {
                    ViewBag.ClassId = new SelectList(classesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.LevelId == LevelId).Select(x => new { x.Id, Name = x.Name + " (" + x.StudyPlaceName + ")" }).ToList(), "Id", "Name", ClassId);
                }
                else
                {
                    ViewBag.ClassId = new SelectList("");
                }
                model = model.Where(x => x.StudyTypeId == StudyTypeId).ToList();
            }
            else
            {
                ViewBag.LevelId = new SelectList("");
                ViewBag.ClassId = new SelectList("");
            }
            if (LevelId != null && LevelId != Guid.Empty)
            {
                model = model.Where(x => x.LevelId == LevelId).ToList();
            }
            if (ClassId != null && ClassId != Guid.Empty)
            {
                model = model.Where(x => x.ClassId == ClassId).ToList();
            }
            return View(model);
        }
        public ActionResult Create()
        {
            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text");
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name");
            ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name");
            ViewBag.DestrictId = new SelectList("");
            return View("Upsert", new Student());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Student student, HttpPostedFileBase Image1)
        {
            student.Id = Guid.NewGuid();
            if (student.Code == null)
                student.Code = randomCode.GenerateStudentCodeRandom();

            if (Image1 != null)
            {
                student.Image = "/Uploads/Studentes/";

                if (!Directory.Exists(Server.MapPath("~" + student.Image + student.Id)))
                    Directory.CreateDirectory(Server.MapPath("~" + student.Image + student.Id));
                student.Image = student.Image + student.Id + "/" + student.Id + ".jpg";
                Image1.SaveAs(Server.MapPath("~" + student.Image));
            }
            var result = studentsServices.Create(student, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                student.Id = Guid.Empty;

                if (student.BirthDate != null)
                    ViewBag.BirthDate = student.BirthDate;
                if (student.JoiningDate != null)
                    ViewBag.JoiningDate = student.JoiningDate;

                ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", student.GenderId);
                ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", student.RegistrationTypeId);

                if (student.DestrictId != null)
                {
                    var cityid = destrictsServices.GetAll().Where(x => x.Id == student.DestrictId).FirstOrDefault().CityId;
                    ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name", cityid);
                    ViewBag.DestrictId = new SelectList(destrictsServices.GetAll().Where(x => x.CityId == cityid).ToList(), "Id", "Name", student.DestrictId);
                }
                else
                {
                    ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name");
                    ViewBag.DestrictId = new SelectList("");
                }

                TempData["warning"] = result.Message;
                return View("Upsert", student);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var student = studentsServices.Get(Id);

            if (student.BirthDate != null && student.BirthDate != string.Empty && student.BirthDate != "")
                ViewBag.BirthDate = DateTime.Parse(student.BirthDate).ToString("yyyy-MM-dd");
            if (student.JoiningDate != null && student.JoiningDate != string.Empty && student.JoiningDate != "")
                ViewBag.JoiningDate = DateTime.Parse(student.JoiningDate).ToString("yyyy-MM-dd");

            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", student.GenderId);
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", student.RegistrationTypeId);

            if (student.DestrictId != null)
            {
                var cityid = destrictsServices.GetAll().Where(x => x.Id == student.DestrictId).FirstOrDefault().CityId;
                ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name", cityid);
                ViewBag.DestrictId = new SelectList(destrictsServices.GetAll().Where(x => x.CityId == cityid).ToList(), "Id", "Name", student.DestrictId);
            }
            else
            {
                ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name");
                ViewBag.DestrictId = new SelectList("");
            }
            return View("Upsert", student);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Student student, HttpPostedFileBase Image1)
        {
            if (Image1 != null)
            {
                student.Image = "/Uploads/Studentes/";

                if (!Directory.Exists(Server.MapPath("~" + student.Image + student.Id)))
                    Directory.CreateDirectory(Server.MapPath("~" + student.Image + student.Id));
                student.Image = student.Image + student.Id + "/" + student.Id + ".jpg";
                Image1.SaveAs(Server.MapPath("~" + student.Image));
            }
            var result = studentsServices.Edit(student, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                if (student.BirthDate != null)
                    ViewBag.BirthDate = student.BirthDate.ToString();
                if (student.JoiningDate != null)
                    ViewBag.JoiningDate = student.JoiningDate;


                ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", student.GenderId);
                ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name", student.RegistrationTypeId);

                if (student.DestrictId != null)
                {
                    var cityid = destrictsServices.GetAll().Where(x => x.Id == student.DestrictId).FirstOrDefault().CityId;
                    ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name", cityid);
                    ViewBag.DestrictId = new SelectList(destrictsServices.GetAll().Where(x => x.CityId == cityid).ToList(), "Id", "Name", student.DestrictId);
                }
                else
                {
                    ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name");
                    ViewBag.DestrictId = new SelectList("");
                }
                TempData["warning"] = result.Message;
                return View("Upsert", student);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = studentsServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public FileResult DownloadExcel()
        {
            string path = "/Uploads/Excel/StudentsFormat.xlsx";
            return File(path, "application/vnd.ms-excel", "StudentsFormat.xlsx");
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

        [Authorized(ScreenId = "52")]
        public ActionResult Search()
        {
            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text");
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name");
            ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name");
            ViewBag.DestrictId = new SelectList("");
            return View(new Student());
        }
        [HttpPost, ValidateInput(false)]
        [Authorized(ScreenId = "52")]
        public ActionResult Search(Student student, Guid? CityId)
        {
            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text");
            ViewBag.RegistrationTypeId = new SelectList(registrationTypes.GetAll(), "Id", "Name");
            ViewBag.CityId = new SelectList(citiesServices.GetAll(), "Id", "Name");
            ViewBag.DestrictId = new SelectList("");

            var students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            if (student.Code != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Code.Contains(student.Code)).ToList();
                }
                else
                {
                    students = students.Where(x => x.Code.Contains(student.Code)).ToList();
                }
            }
            if (student.Name != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Name.Contains(student.Name)).ToList();
                }
                else
                {
                    students = students.Where(x => x.Name.Contains(student.Name)).ToList();
                }
            }
            if (student.GenderId != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.GenderId == student.GenderId).ToList();
                }
                else
                {
                    students = students.Where(x => x.GenderId == student.GenderId).ToList();
                }
            }
            if (student.RegistrationTypeId != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.RegistrationTypeId == student.RegistrationTypeId).ToList();
                }
                else
                {
                    students = students.Where(x => x.RegistrationTypeId == student.RegistrationTypeId).ToList();
                }
            }
            if (CityId != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.CityId == CityId).ToList();
                }
                else
                {
                    students = students.Where(x => x.CityId == CityId).ToList();
                }
            }
            if (student.DestrictId != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.DestrictId == student.DestrictId).ToList();
                }
                else
                {
                    students = students.Where(x => x.DestrictId == student.DestrictId).ToList();
                }
            }
            if (student.Phone != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Phone.Contains(student.Phone)).ToList();
                }
                else
                {
                    students = students.Where(x => x.Phone.Contains(student.Phone)).ToList();
                }
            }
            if (student.JoiningDate != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.JoiningDate == student.JoiningDate).ToList();
                }
                else
                {
                    students = students.Where(x => x.JoiningDate == student.JoiningDate).ToList();
                }
            }
            ViewBag.Students = students;

            return View(student);
        }

        [Authorized(ScreenId = "52")]
        public ActionResult getDestrictsSearch(Guid Id)
        {
            if (Id == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var model = destrictsServices.GetAll().Where(x => x.CityId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [Authorized(ScreenId = "53")]
        public ActionResult getDestrictsReport(Guid Id)
        {
            if (Id == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var model = destrictsServices.GetAll().Where(x => x.CityId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [Authorized(ScreenId = "53")]
        public ActionResult Reports()
        {
            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name");
            ViewBag.StudentId = new SelectList(studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
            ViewBag.Count = 0;
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [Authorized(ScreenId = "53")]
        public ActionResult Reports(Guid? StudyYearId = null, Guid? StudentId = null)
        {
            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name", StudyYearId);
            var students = studentsServices.GetAllReport(StudyYearId.Value, (Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);

            if (StudentId != null)
            {
                students = students.Where(x => x.Id == StudentId).ToList();
                ViewBag.StudentId = new SelectList(studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name", StudentId);
            }
            else
            {
                ViewBag.StudentId = new SelectList(studentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
            }
            ViewBag.Students = students;
            ViewBag.Count = students.Count();
            return View();
        }



        public ActionResult getDestricts(Guid Id)
        {
            if (Id == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var model = destrictsServices.GetAll().Where(x => x.CityId == Id).Select(x => new { x.Id, x.Name }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getPaidDetails(Guid StudentId, Guid StudyYearId,string Type)
        {
            if (StudentId == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var students = studentsServices.GetAllReport(StudyYearId, (Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            var model = students.Where(x => x.Id == StudentId).FirstOrDefault().PaidDetails.Where(x=>x.Paided== Type).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getAttendanceDetails(Guid StudentId, Guid StudyYearId, bool Type)
        {
            if (StudentId == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var students = studentsServices.GetAllReport(StudyYearId, (Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            var model = students.Where(x => x.Id == StudentId).FirstOrDefault().AttendanceDetails.Where(x => x.IsAttend == Type).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        List<ListItem> Genders()
        {
            Array values = Enum.GetValues(typeof(Gender));
            List<ListItem> items = new List<ListItem>(values.Length);
            var count = 1;
            foreach (var i in values)
            {
                items.Add(new ListItem
                {
                    Text = Enum.GetName(typeof(Gender), i),
                    Value = count.ToString()
                });
                count++;
            }
            return items;
        }

        #region Import MCQ Question Excel

        public ActionResult ImportExcel(HttpPostedFileBase upload)
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
                        return RedirectToAction("Index");
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
                        return RedirectToAction("Index");
                    }

                    reader.Close();
                    reader.Dispose();
                    for (int i = 0; i < model.Rows.Count; i++)
                    {
                        Student student = new Student();
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
                    TempData["success"] = "تم حفظ البيانات بنجاح";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["warning"] = "البيانات المدخلة غير صحيحة";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        #endregion

    }
}
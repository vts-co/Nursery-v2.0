using NurseryProject.Authorization;
using NurseryProject.Dtos.Students;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Cities;
using NurseryProject.Services.Destricts;
using NurseryProject.Services.RegistrationTypes;
using NurseryProject.Services.Students;
using NurseryProject.Services.StudyYears;
using NurseryProject.Utilities;
using System;
using System.Collections.Generic;
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

        // GET: Cities
        public ActionResult Index()
        {
            var model = studentsServices.GetAll();
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
                    ViewBag.BirthDate = student.BirthDate.Value.ToString("yyyy-MM-dd");
                if (student.JoiningDate != null)
                    ViewBag.JoiningDate = student.JoiningDate.Value.ToString("yyyy-MM-dd");

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

            if (student.BirthDate != null)
                ViewBag.BirthDate = student.BirthDate.Value.ToString("yyyy-MM-dd");
            if (student.JoiningDate != null)
                ViewBag.JoiningDate = student.JoiningDate.Value.ToString("yyyy-MM-dd");

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
                    ViewBag.BirthDate = student.BirthDate.Value.ToString("yyyy-MM-dd");
                if (student.JoiningDate != null)
                    ViewBag.JoiningDate = student.JoiningDate.Value.ToString("yyyy-MM-dd");


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

            var students = studentsServices.GetAll();
            if (student.Code != null)
            {
                if (students.Count() == 0)
                {
                    students = studentsServices.GetAll().Where(x => x.Code.Contains(student.Code)).ToList();
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
                    students = studentsServices.GetAll().Where(x => x.Name.Contains(student.Name)).ToList();
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
                    students = studentsServices.GetAll().Where(x => x.GenderId == student.GenderId).ToList();
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
                    students = studentsServices.GetAll().Where(x => x.RegistrationTypeId == student.RegistrationTypeId).ToList();
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
                    students = studentsServices.GetAll().Where(x => x.CityId == CityId).ToList();
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
                    students = studentsServices.GetAll().Where(x => x.DestrictId == student.DestrictId).ToList();
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
                    students = studentsServices.GetAll().Where(x => x.Phone.Contains(student.Phone)).ToList();
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
                    students = studentsServices.GetAll().Where(x => x.JoiningDate == student.JoiningDate.Value.ToString()).ToList();
                }
                else
                {
                    students = students.Where(x => x.JoiningDate == student.JoiningDate.Value.ToString()).ToList();
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
            ViewBag.StudentId=new SelectList(studentsServices.GetAll(), "Id", "Name");
            ViewBag.Count = 0;
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [Authorized(ScreenId = "53")]
        public ActionResult Reports(Guid? StudyYearId = null,Guid? StudentId=null)
        {
            ViewBag.StudyYearId = new SelectList(studyYearsServices.GetAll(), "Id", "Name", StudyYearId);
            var students = studentsServices.GetAllReport(StudyYearId.Value);

            if (StudentId!=null)
            {
                students = students.Where(x => x.Id == StudentId).ToList();
                ViewBag.StudentId = new SelectList(studentsServices.GetAll(), "Id", "Name", StudentId);
            }
            else
            {
                ViewBag.StudentId = new SelectList(studentsServices.GetAll(), "Id", "Name");
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
    }
}
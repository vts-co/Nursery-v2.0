using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.Jops;
using NurseryProject.Services.RegistrationTypes;
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
    [Authorized(Role = Role.SystemAdmin)]

    public class EmployeesController : Controller
    {
        EmployeesServices employeesServices = new EmployeesServices();
        GenerateRandomCode randomCode = new GenerateRandomCode();
        RegistrationTypesServices registrationTypes = new RegistrationTypesServices();
        DepartmentsServices departmentsServices = new DepartmentsServices();
        JopsServices jopsServices = new JopsServices();

        // GET: Cities
        public ActionResult Index()
        {
            var model = employeesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text");
            ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");
            ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name");
            ViewBag.JopId = new SelectList("");
            return View("Upsert", new Employee());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Employee employee, HttpPostedFileBase Image1)
        {
            if (employee != null)
            {
                employee.Image = "/Uploads/Employees/";

                if (!Directory.Exists(Server.MapPath("~" + employee.Image + employee.Id)))
                    Directory.CreateDirectory(Server.MapPath("~" + employee.Image + employee.Id));
                employee.Image = employee.Image + employee.Id + "/" + employee.Id + ".jpg";
                Image1.SaveAs(Server.MapPath("~" + employee.Image));
            }
            employee.Id = Guid.NewGuid();
            if (employee.Code == null)
                employee.Code = randomCode.GenerateEmployeeCodeRandom();

            var result = employeesServices.Create(employee, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                employee.Id = Guid.Empty;

                if (employee.BirthDate != null)
                    ViewBag.BirthDate = employee.BirthDate.Value.ToString("yyyy-MM-dd");
                if (employee.JoiningDate != null)
                    ViewBag.JoiningDate = employee.JoiningDate.Value.ToString("yyyy-MM-dd");

                ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", employee.GenderId);
                ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");

                if (employee.JopId != null)
                {
                    var DepartmentId = jopsServices.GetAll().Where(x => x.Id == employee.JopId).FirstOrDefault().DepartmentId;
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name", DepartmentId);
                    ViewBag.JopId = new SelectList(jopsServices.GetAll().Where(x => x.DepartmentId == DepartmentId).ToList(), "Id", "Name", employee.JopId);
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name");
                    ViewBag.JopId = new SelectList("");
                }

                TempData["warning"] = result.Message;
                return View("Upsert", employee);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employee = employeesServices.Get(Id);

            if (employee.BirthDate != null)
                ViewBag.BirthDate = employee.BirthDate.Value.ToString("yyyy-MM-dd");
            if (employee.JoiningDate != null)
                ViewBag.JoiningDate = employee.JoiningDate.Value.ToString("yyyy-MM-dd");

            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", employee.GenderId);
            ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");

            if (employee.JopId != null)
            {
                var DepartmentId = jopsServices.GetAll().Where(x => x.Id == employee.JopId).FirstOrDefault().DepartmentId;
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name", DepartmentId);
                ViewBag.JopId = new SelectList(jopsServices.GetAll().Where(x => x.DepartmentId == DepartmentId).ToList(), "Id", "Name", employee.JopId);
            }
            else
            {
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name");
                ViewBag.JopId = new SelectList("");
            }
            return View("Upsert", employee);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Employee employee, HttpPostedFileBase Image1)
        {

            var result = employeesServices.Edit(employee, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                if (employee.BirthDate != null)
                    ViewBag.BirthDate = employee.BirthDate.Value.ToString("yyyy-MM-dd");
                if (employee.JoiningDate != null)
                    ViewBag.JoiningDate = employee.JoiningDate.Value.ToString("yyyy-MM-dd");

                ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", employee.GenderId);
                ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");

                if (employee.JopId != null)
                {
                    var DepartmentId = jopsServices.GetAll().Where(x => x.Id == employee.JopId).FirstOrDefault().DepartmentId;
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name", DepartmentId);
                    ViewBag.JopId = new SelectList(jopsServices.GetAll().Where(x => x.DepartmentId == DepartmentId).ToList(), "Id", "Name", employee.JopId);
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name");
                    ViewBag.JopId = new SelectList("");
                }
                TempData["warning"] = result.Message;
                return View("Upsert", employee);
            }
        }
        public ActionResult Reports()
        {
            ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Reports(Guid? DepartmentId=null, bool All = false)
        {
            if (DepartmentId != null && DepartmentId != Guid.Empty)
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name",DepartmentId);
            else
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll(), "Id", "Name", DepartmentId);


            var employees = employeesServices.GetAll();
            if (All)
            {
                ViewBag.Reports = employees;
                ViewBag.Count = employees.Count();
                return View();
            }
            if (DepartmentId != null && DepartmentId != Guid.Empty)
            {
                employees = employees.Where(x=>x.DepartmentId==DepartmentId).ToList();
                ViewBag.Reports = employees;
                ViewBag.Count = employees.Count();
                return View();
            }
            ViewBag.Reports = employees;
            ViewBag.Count = employees.Count();
            return View();
        }

        public ActionResult Delete(Guid Id)
        {
            var result = employeesServices.Delete(Id, (Guid)TempData["UserId"]);
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
        public ActionResult getJops(Guid Id)
        {
            if (Id == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var model = jopsServices.GetAll().Where(x => x.DepartmentId == Id).Select(x => new { x.Id, x.Name }).ToList();
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
        List<ListItem> MaritalStatus()
        {
            Array values = Enum.GetValues(typeof(MaritalStatus));
            List<ListItem> items = new List<ListItem>(values.Length);
            var count = 1;
            foreach (var i in values)
            {
                items.Add(new ListItem
                {
                    Text = Enum.GetName(typeof(MaritalStatus), i),
                    Value = count.ToString()
                });
                count++;
            }
            return items;
        }
    }
}
using ExcelDataReader;
using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.Departments;
using NurseryProject.Services.Employees;
using NurseryProject.Services.Jops;
using NurseryProject.Services.EmployeesRegistrationTypes;
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
    [Authorized(ScreenId = "32")]

    public class EmployeesController : Controller
    {
        EmployeesServices employeesServices = new EmployeesServices();
        GenerateRandomCode randomCode = new GenerateRandomCode();
        RegistrationTypesServices registrationTypes = new RegistrationTypesServices();
        DepartmentsServices departmentsServices = new DepartmentsServices();
        JopsServices jopsServices = new JopsServices();
        RegistrationTypesServices registrationTypesServices = new RegistrationTypesServices();

        // GET: Cities
        public ActionResult Index()
        {
            ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
            ViewBag.JopId = new SelectList("");
            var model = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
            return View(model);
        }
        public ActionResult Create()
        {
            var model = registrationTypesServices.GetAll();

            ViewBag.RegistrationTypeId = new SelectList(model, "Id", "Name");
            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text");
            ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");
            ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
            ViewBag.JopId = new SelectList("");
            return View("Upsert", new Employee());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Employee employee, HttpPostedFileBase Image1)
        {
            if (employee != null&& Image1!=null)
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
                    ViewBag.BirthDate = employee.BirthDate;
                if (employee.JoiningDate != null)
                    ViewBag.JoiningDate = employee.JoiningDate;

                ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", employee.GenderId);
                ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");

                var model = registrationTypesServices.GetAll();
                ViewBag.RegistrationTypeId = new SelectList(model, "Id", "Name", employee.RegistrationTypeId);

                if (employee.JopId != null)
                {
                    var DepartmentId = jopsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == employee.JopId).FirstOrDefault().DepartmentId;
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name", DepartmentId);
                    ViewBag.JopId = new SelectList(jopsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.DepartmentId == DepartmentId).ToList(), "Id", "Name", employee.JopId);
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
                    ViewBag.JopId = new SelectList("");
                }

                TempData["warning"] = result.Message;
                return View("Upsert", employee);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var employee = employeesServices.Get(Id);

            if (employee.BirthDate != null&& employee.BirthDate !=string.Empty && employee.BirthDate != "")
                ViewBag.BirthDate = DateTime.Parse(employee.BirthDate).ToString("yyyy-MM-dd");
            if (employee.JoiningDate != null && employee.JoiningDate != string.Empty&& employee.JoiningDate != "")
                ViewBag.JoiningDate =DateTime.Parse(employee.JoiningDate).ToString("yyyy-MM-dd");

            ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", employee.GenderId);
            ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");

            var model = registrationTypesServices.GetAll();
            ViewBag.RegistrationTypeId = new SelectList(model, "Id", "Name", employee.RegistrationTypeId);


            if (employee.JopId != null)
            {
                var DepartmentId = jopsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == employee.JopId).FirstOrDefault().DepartmentId;
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name", DepartmentId);
                ViewBag.JopId = new SelectList(jopsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.DepartmentId == DepartmentId).ToList(), "Id", "Name", employee.JopId);
            }
            else
            {
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
                ViewBag.JopId = new SelectList("");
            }
            return View("Upsert", employee);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Employee employee, HttpPostedFileBase Image1)
        {
            if (employee != null && Image1 != null)
            {
                employee.Image = "/Uploads/Employees/";

                if (!Directory.Exists(Server.MapPath("~" + employee.Image + employee.Id)))
                    Directory.CreateDirectory(Server.MapPath("~" + employee.Image + employee.Id));
                employee.Image = employee.Image + employee.Id + "/" + employee.Id + ".jpg";
                Image1.SaveAs(Server.MapPath("~" + employee.Image));
            }

            var result = employeesServices.Edit(employee, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                if (employee.BirthDate != null)
                    ViewBag.BirthDate = employee.BirthDate;
                if (employee.JoiningDate != null)
                    ViewBag.JoiningDate = employee.JoiningDate;

                ViewBag.GenderId = new SelectList(Genders(), "Value", "Text", employee.GenderId);
                ViewBag.MaritalStateId = new SelectList(MaritalStatus(), "Value", "Text");

                var model = registrationTypesServices.GetAll();
                ViewBag.RegistrationTypeId = new SelectList(model, "Id", "Name", employee.RegistrationTypeId);


                if (employee.JopId != null)
                {
                    var DepartmentId = jopsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.Id == employee.JopId).FirstOrDefault().DepartmentId;
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name", DepartmentId);
                    ViewBag.JopId = new SelectList(jopsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.DepartmentId == DepartmentId).ToList(), "Id", "Name", employee.JopId);
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
                    ViewBag.JopId = new SelectList("");
                }
                TempData["warning"] = result.Message;
                return View("Upsert", employee);
            }
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


        #region Import MCQ Question Excel

        public ActionResult ImportExcel(HttpPostedFileBase upload,Guid DepartmentId,Guid JopId)
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
                        Employee student = new Employee();
                        student.Id = Guid.NewGuid();
                        if (model.Rows[i][0].ToString() == "" || model.Rows[i][0].ToString() == null)
                            student.Code = randomCode.GenerateStudentCodeRandom();
                        else if (employeesServices.CodeExist(model.Rows[i][0].ToString()))
                            student.Code = randomCode.GenerateStudentCodeRandom();
                        else
                            student.Code = model.Rows[i][0].ToString();

                        student.Name = model.Rows[i][1].ToString();

                        student.BirthDate = model.Rows[i][2].ToString();

                        if (model.Rows[i][3].ToString() == "ذكر")
                            student.GenderId = (int)Gender.ذكر;
                        else if (model.Rows[i][3].ToString() == "انثي")
                            student.GenderId = (int)Gender.انثي;

                        student.Qualification = model.Rows[i][4].ToString();

                        student.JopId = JopId;


                        student.Phone = model.Rows[i][5].ToString();
                        student.Address = model.Rows[i][6].ToString();
                       

                        student.JoiningDate = model.Rows[i][7].ToString();
                        student.WorkDayCost = float.Parse(model.Rows[i][8].ToString());
                        student.Notes = model.Rows[i][9].ToString();

                        employeesServices.Create(student, (Guid)TempData["UserId"]);

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

        [Authorized(ScreenId = "63")]
        public ActionResult Reports()
        {
            ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name");
            return View();
        }
        [HttpPost]
        [Authorized(ScreenId = "63")]

        public ActionResult Reports(Guid? DepartmentId=null, bool All = false)
        {
            if (DepartmentId != null && DepartmentId != Guid.Empty)
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name",DepartmentId);
            else
                ViewBag.DepartmentId = new SelectList(departmentsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]), "Id", "Name", DepartmentId);


            var employees = employeesServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]);
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

        public ActionResult getJops(Guid Id)
        {
            if (Id == null)
            {
                return Json(new SelectList(""), JsonRequestBehavior.AllowGet);
            }
            var model = jopsServices.GetAll((Guid)TempData["UserId"], (Guid)TempData["EmployeeId"], (Role)TempData["RoleId"]).Where(x => x.DepartmentId == Id).Select(x => new { x.Id, x.Name }).ToList();
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
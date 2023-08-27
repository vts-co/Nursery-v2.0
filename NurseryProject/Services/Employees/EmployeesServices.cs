using NurseryProject.Dtos.Employees;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Employees
{
    public class EmployeesServices
    {
        public List<EmployeesDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Employees.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Id == EmployeeId || x.EmployeeClasses.Any(z=>z.IsDeleted==false&&z.Class.StudyPlace.BuildingSupervisors.Any(y => y.IsDeleted == false && y.EmployeeId == EmployeeId)))).OrderBy(x => x.CreatedOn).Select(x => new EmployeesDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Phone = x.Phone,
                    Address = x.Address,
                    BirthDate = x.BirthDate.ToString(),
                    GenderId = x.GenderId.Value,
                    GenderName = x.GenderId == 1 ? "ذكر" : "انثي",
                    JoiningDate = x.JoiningDate.ToString(),
                    DepartmentId = x.Jop.DepartmentId.Value,
                    DepartmentName = x.Jop.Department.Name,
                    JopId = x.JopId.Value,
                    Image=x.Image,
                    JopName = x.Jop.Name,
                    Qualification=x.Qualification,
                    WorkDayCost = x.WorkDayCost.Value.ToString(),
                    //MaritalStateId = (int)x.MaritalStateId,
                    Notes = x.Notes
                }).ToList();
                foreach (var item in model)
                {
                    if (item.BirthDate != null && item.BirthDate.Trim() != "")
                        item.BirthDate = DateTime.Parse(item.BirthDate).ToString("yyyy-MM-dd");
                    if (item.JoiningDate != null && item.JoiningDate != string.Empty && item.JoiningDate != "")
                        item.JoiningDate = DateTime.Parse(item.JoiningDate).ToString("yyyy-MM-dd");
                }
                foreach (var item in model)
                {
                    if (item.MaritalStateId == 1)
                        item.MaritalStateName = "اعزب";
                    if (item.MaritalStateId == 2)
                        item.MaritalStateName = "متزوج";
                    if (item.MaritalStateId == 3)
                        item.MaritalStateName = "مطلق";
                    if (item.MaritalStateId == 4)
                        item.MaritalStateName = "ارمل";
                }
                return model;
            }
        }
        public Employee Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Employees.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public bool CodeExist(string code)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Employees.Where(x => x.IsDeleted == false && x.Code == code).OrderBy(x => x.CreatedOn).FirstOrDefault();
                if (model != null)
                    return true;
                return false;
                        
            }
        }
        public ResultDto<Employee> Create(Employee model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Employee>();
                var Oldmodel = dbContext.Employees.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا الموظف موجود بالفعل";
                    return result;
                }
                var Oldmodel2 = dbContext.Employees.Where(x => x.Code == model.Code && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel2 != null)
                {
                    result.Result = Oldmodel2;
                    result.IsSuccess = false;
                    result.Message = "هذا الكود موجود لم يمكن استخدامه";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Employees.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Employee> Edit(Employee model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Employee>();
                var Oldmodel = dbContext.Employees.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الموظف موجود بالفعل";
                    return result;
                }
                var Oldmodel2 = dbContext.Employees.Where(x => x.Code == model.Code&&x.Id!=model.Id && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel2 != null)
                {
                    result.Result = Oldmodel2;
                    result.IsSuccess = false;
                    result.Message = "هذا الكود موجود لم يمكن استخدامه";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                if (model.Code != null)
                    Oldmodel.Code = model.Code;

                Oldmodel.Name = model.Name;
                Oldmodel.Phone = model.Phone;
                Oldmodel.Address = model.Address;
                Oldmodel.BirthDate = model.BirthDate;
                Oldmodel.GenderId = model.GenderId;
                Oldmodel.MaritalStateId = model.MaritalStateId;
                Oldmodel.WorkDayCost = model.WorkDayCost.Value;
                Oldmodel.JoiningDate = model.JoiningDate;
                Oldmodel.JopId = model.JopId.Value;
                Oldmodel.Notes = model.Notes;
                Oldmodel.Qualification = model.Qualification;

                if (model.Image != null)
                {
                    Oldmodel.Image = model.Image;
                }
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Employee> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Employee>();
                var Oldmodel = dbContext.Employees.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الموظف غير موجود ";
                    return result;
                }
                if (Oldmodel.Revenues.Any(y => y.IsDeleted == false) || Oldmodel.Expenses.Any(y => y.IsDeleted == false) || Oldmodel.EmployeeClasses.Any(y => y.IsDeleted == false) || Oldmodel.EmployeesDiscounts.Any(y => y.IsDeleted == false) || Oldmodel.EmployeesIncreases.Any(y => y.IsDeleted == false) || Oldmodel.EmployeesReceipts.Any(y => y.IsDeleted == false) || Oldmodel.EmployeesVacations.Any(y => y.IsDeleted == false) || Oldmodel.EmployeesWorkShifts.Any(y => y.IsDeleted == false) || Oldmodel.ClassesLeaders.Any(y => y.IsDeleted == false) || Oldmodel.BuildingSupervisors.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الموظف لا يمكن حذفه  ";
                    return result;
                }
                Oldmodel.IsDeleted = true;
                Oldmodel.DeletedOn = DateTime.UtcNow;
                Oldmodel.DeletedBy = UserId;
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حذف البيانات بنجاح";
                return result;
            }
        }
    }
}
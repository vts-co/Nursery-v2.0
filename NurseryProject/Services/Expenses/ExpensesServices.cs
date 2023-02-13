using NurseryProject.Dtos.Expenses;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Expenses
{
    public class ExpensesServices
    {
        public List<ExpensesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Expenses.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new ExpensesDto
                {
                    Id = x.Id,
                    StudyPlaceId = x.StudyPlaceId.Value,
                    StudyPlaceName = x.StudyPlace.Name,
                    StudyYearId = x.StudyYearId.Value,
                    StudyYearName = x.StudyYear.Name,
                    ExpenseTypePatentId=x.ExpensesType.ParentId.Value,
                    ExpenseTypePatentName=x.ExpensesType.ExpensesType1.Name,
                    ExpenseTypeId = x.ExpenseTypeId.Value,
                    ExpenseTypeName = x.ExpensesType.Name,
                    EmployeeId = x.EmployeeId==null? Guid.Empty:x.EmployeeId.Value,
                    EmployeeName = x.EmployeeId == null ? "": x.Employee.Name,
                    Value = x.ExpenseValue,
                    Date = x.ExpenseDate.Value,
                    Notes = x.Notes,
                    EmployeesReceiptId = x.EmployeesReceiptId == null ? Guid.Empty : x.EmployeesReceiptId.Value

                }).ToList();
                return model;
            }
        }
        public Expens Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Expenses.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Expens> Create(Expens model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Expens>();

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Expenses.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Expens> Edit(Expens model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Expens>();
                var Oldmodel = dbContext.Expenses.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.StudyPlaceId = model.StudyPlaceId;
                Oldmodel.StudyYearId = model.StudyYearId;
                Oldmodel.ExpenseTypeId = model.ExpenseTypeId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.ExpenseValue = model.ExpenseValue;
                Oldmodel.ExpenseDate = model.ExpenseDate;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Expens> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Expens>();
                var Oldmodel = dbContext.Expenses.Find(Id);

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
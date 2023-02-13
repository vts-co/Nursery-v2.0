using NurseryProject.Dtos.EmployeesDiscounts;
using NurseryProject.Dtos.EmployeesIncreases;
using NurseryProject.Dtos.EmployeesReceipt;
using NurseryProject.Models;
using NurseryProject.Services.EmployeeClasses;
using NurseryProject.Services.Employees;
using NurseryProject.Services.EmployeesDiscounts;
using NurseryProject.Services.EmployeesIncreases;
using NurseryProject.Services.Expenses;
using NurseryProject.Services.StudyYears;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeesReceipt
{
    public class EmployeesReceiptServices
    {
        EmployeesDiscountsServices employeesDiscountsServices = new EmployeesDiscountsServices();
        EmployeesIncreasesServices employeesIncreasesServices = new EmployeesIncreasesServices();
        ExpensesServices expensesServices = new ExpensesServices();
        EmployeeClassesServices employeeClassesServices = new EmployeeClassesServices();
        public EmployeesReceiptDto GetAll(string date, Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var attendCount = 0;
                var noattendCount = 0;
                var totalCostCount = 0;
                var RecieptCount = 0;
                var Date = "";
                var Paid = "";

                var attend = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == true && y.EmployeesWorkShift.EmployeeId == Id).ToList();
                var noattend = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == false && y.EmployeesWorkShift.EmployeeId == Id).ToList();
                var totalCost = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == true && y.EmployeesWorkShift.EmployeeId == Id).ToList();
                var Reciept = dbContext.EmployeesReceipts.Where(y => y.IsDeleted == false&&y.Month.ToString().Contains(date) && y.EmployeeId == Id).ToList();

                foreach (var item in attend)
                {
                    if (item.Date.Value.ToString("yyyy-MM") == date)
                    {
                        attendCount += 1;
                    }
                }
                foreach (var item in noattend)
                {
                    if (item.Date.Value.ToString("yyyy-MM") == date)
                    {
                        noattendCount += 1;
                    }
                }
                foreach (var item in totalCost)
                {
                    if (item.Date.Value.ToString("yyyy-MM") == date)
                    {
                        totalCostCount += 1;
                    }
                }
                foreach (var item in Reciept)
                {
                    if (item.Date.Value.ToString("yyyy-MM") == date)
                    {
                        RecieptCount += 1;
                        Date = item.Date.Value.ToString("yyyy-MM-dd");
                        Paid = item.FinalTotalAmount.Value.ToString();
                    }
                }
                var discounts = dbContext.EmployeesDiscounts.Where(y => y.DiscountDate.Value.ToString().Contains(date) && y.EmployeeId == Id).ToList();
                var increases = dbContext.EmployeesIncreases.Where(z => z.IncreaseDate.Value.ToString().Contains(date) && z.EmployeeId == Id).ToList();
                var model = dbContext.Employees.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).Select(x => new EmployeesReceiptDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Phone = x.Phone,
                    Department = x.Jop.Department.Name,
                    Jop = x.Jop.Name,
                    Month = date,
                    WorkDayCost = x.WorkDayCost.ToString(),
                    EmployeesAttended = attendCount,
                    EmployeesNoAttended = noattendCount,
                    TotalCost = (totalCostCount * x.WorkDayCost).ToString(),
                    EmployeesDiscount = dbContext.EmployeesDiscounts.Where(y => y.DiscountDate.Value.ToString().Contains(date) && y.EmployeeId == x.Id).Select(y => new Dtos.EmployeesReceipt.EmployeesDiscounts
                    {
                        Id = y.Id,
                        DiscountTypeId = y.DiscountTypeId.Value,
                        DiscountTypeName = y.DiscountsType.Name,
                        Date = y.DiscountDate.Value,
                        EmployeeId = y.EmployeeId.Value,
                        EmployeeName = y.Employee.Name,
                        Value = y.DiscountValue.Value.ToString(),
                        Reason = y.DiscountReason
                    }).ToList(),
                    EmployeesIncreases = dbContext.EmployeesIncreases.Where(z => z.IncreaseDate.Value.ToString().Contains(date) && z.EmployeeId == x.Id).Select(z => new Dtos.EmployeesReceipt.EmployeesIncreases
                    {
                        Id = z.Id,
                        IncreaseTypeId = z.IncreaseTypeId.Value,
                        IncreaseTypeName = z.IncreasesType.Name,
                        Date = z.IncreaseDate.Value,
                        EmployeeId = z.EmployeeId.Value,
                        EmployeeName = z.Employee.Name,
                        Value = z.IncreaseValue.Value.ToString(),
                        Reason = z.IncreaseReason
                    }).ToList(),
                    TotalDiscountCost = dbContext.EmployeesDiscounts.Where(y => y.DiscountDate.Value.ToString().Contains(date) && y.EmployeeId == x.Id).Sum(a => a.DiscountValue.Value).ToString(),
                    TotalIncreasesCost = dbContext.EmployeesIncreases.Where(z => z.IncreaseDate.Value.ToString().Contains(date) && z.EmployeeId == x.Id).Sum(b => b.IncreaseValue.Value).ToString(),
                    FinalTotalCost = ((totalCostCount * x.WorkDayCost) +
                    dbContext.EmployeesIncreases.Where(z => z.IncreaseDate.Value.ToString().Contains(date) && z.EmployeeId == x.Id).Sum(c => c.IncreaseValue) -
                    dbContext.EmployeesDiscounts.Where(y => y.DiscountDate.Value.ToString().Contains(date) && y.EmployeeId == x.Id).Sum(d => d.DiscountValue)).ToString(),
                    Reciept = RecieptCount.ToString(),
                    Date= Date,
                    Paid=Paid
                }).FirstOrDefault();
                if (model.TotalDiscountCost == "")
                {
                    model.TotalDiscountCost = "0";
                }
                if (model.TotalIncreasesCost == "")
                {
                    model.TotalIncreasesCost = "0";
                }
                model.FinalTotalCost = ((totalCostCount * float.Parse(model.WorkDayCost)) -
                    float.Parse(model.TotalDiscountCost) +
                    float.Parse(model.TotalIncreasesCost)).ToString();

                return model;
            }
        }

        public ResultDto<Models.EmployeesReceipt> Create(string date,string month, Guid EmployeeId, float Total, float TotalDiscountCost, float TotalIncreasesCost, float Final, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesReceipt>();
                Models.EmployeesReceipt model = new Models.EmployeesReceipt();

                model.Id = Guid.NewGuid();
                model.EmployeeId = EmployeeId;
                model.TotalAmount = Total;
                model.TotalAdds = TotalIncreasesCost;
                model.TotalDiscount = TotalDiscountCost;
                model.Date = DateTime.Parse(date).Date;
                model.Month = DateTime.Parse(month).Date;
                model.FinalTotalAmount = Final;
                
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.EmployeesReceipts.Add(model);
                dbContext.SaveChanges();

                var employeeClasses= employeeClassesServices.GetAll().Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
                var Expense = new Models.Expens
                {
                    Id = Guid.NewGuid(),
                    StudyYearId = employeeClasses.StudyYearId,
                    StudyPlaceId = employeeClasses.StudyPlaceId,
                    ExpenseTypeId = Guid.Parse("20A6A59E-088E-4E6A-AA27-8A15F051B1DE"),
                    ExpenseValue = Final.ToString(),
                    ExpenseDate = DateTime.Parse(date).Date,
                    Notes = "راتب الموظف " + employeeClasses.EmployeeName,
                    EmployeeId = null,
                    EmployeesReceiptId = model.Id
                };
                expensesServices.Create(Expense, UserId);

                result.Result = model;
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesReceipt> Delete(string Date, Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesReceipt>();
                var Oldmodel = dbContext.EmployeesReceipts.Where(x => x.Date.ToString().Contains(Date) && x.IsDeleted == false && x.EmployeeId == Id).FirstOrDefault();
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا القبض غير موجود ";
                    return result;
                }

                Oldmodel.IsDeleted = true;
                Oldmodel.DeletedOn = DateTime.UtcNow;
                Oldmodel.DeletedBy = UserId;
                dbContext.SaveChanges();

                var exp = expensesServices.GetAll().Where(x => x.EmployeesReceiptId == Oldmodel.Id).FirstOrDefault();
                expensesServices.Delete(exp.Id, UserId);

                result.Result = Oldmodel;
                result.IsSuccess = true;
                result.Message = "تم حذف البيانات بنجاح";
                return result;
            }
        }
    }
}
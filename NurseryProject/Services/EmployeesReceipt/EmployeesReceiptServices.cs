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
        ExpensesServices expensesServices = new ExpensesServices();
        EmployeeClassesServices employeeClassesServices = new EmployeeClassesServices();
        EmployeesServices employeesServices = new EmployeesServices();
        public List<EmployeesReceiptDto> GetAllEmployeesReceipts()
        {
            var attendCount = 0;
            var noattendCount = 0;
           
            using (var dbContext = new almohandes_DbEntities())
            {
                
                var model = dbContext.EmployeesReceipts.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new EmployeesReceiptDto
                {
                    Id = x.Id,
                    EmployeeId=x.Employee.Id,
                    Code = x.Employee.Code,
                    Name = x.Employee.Name,
                    Phone = x.Employee.Phone,
                    Department = x.Employee.Jop.Department.Name,
                    Jop = x.Employee.Jop.Name,
                    Month = x.Month.Value.ToString(),
                    Date=x.Date.Value.ToString(),
                    WorkDayCost = x.Employee.WorkDayCost.ToString(),
                    EmployeesAttended = 0,
                    EmployeesNoAttended = 0,
                    TotalCost = x.TotalAmount.Value.ToString(),
                    TotalDiscountCost=x.TotalDiscount.Value.ToString(),
                    TotalIncreasesCost=x.TotalAdds.Value.ToString(),
                    FinalTotalCost=x.FinalTotalAmount.ToString()
                }).ToList();

                foreach (var item in model)
                {
                    item.Month = DateTime.Parse(item.Month).ToString("yyyy-MM");
                    var attend = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == true && y.EmployeesWorkShift.EmployeeId == item.EmployeeId).ToList();

                    foreach (var item2 in attend)
                    {
                        if (item2.Date.Value.ToString("yyyy-MM") == DateTime.Parse(item.Month).ToString("yyyy-MM"))
                        {
                            attendCount += 1;
                        }
                    }
                    item.EmployeesAttended = attendCount;

                    var noattend = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == false && y.EmployeesWorkShift.EmployeeId == item.EmployeeId).ToList();

                    foreach (var item3 in noattend)
                    {

                        if (item3.Date.Value.ToString("yyyy-MM") == DateTime.Parse(item.Month).ToString("yyyy-MM"))
                        {
                            noattendCount += 1;
                        }
                    }
                    item.EmployeesNoAttended = noattendCount;
                }
                return model;
            }
        }

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
                var StudyYear = "";
                var StudyPlace = "";

                var attend = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == true && y.EmployeesWorkShift.EmployeeId == Id).ToList();
                var noattend = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == false  && y.EmployeesWorkShift.EmployeeId == Id).ToList();
                var totalCost = dbContext.EmployeesAttendances.Where(y => y.IsDeleted == false && y.IsAttend == true && y.EmployeesWorkShift.EmployeeId == Id).ToList();
                var Reciept = dbContext.EmployeesReceipts.Where(y => y.IsDeleted == false && y.EmployeeId == Id).FirstOrDefault();
                var Expense = new Expens();
                if (Reciept != null)
                {
                    Expense = dbContext.Expenses.Where(y => y.IsDeleted == false && y.EmployeesReceiptId == Reciept.Id).FirstOrDefault();
                    if(Expense!=null)
                    {
                        StudyPlace = Expense.StudyPlace.Name;
                        StudyYear = Expense.StudyYear.Name;
                    }
                    
                }

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
                if (Reciept != null)
                {
                    if (Reciept.Date.Value.ToString("yyyy-MM") == date)
                    {
                        RecieptCount += 1;
                        Date = Reciept.Date.Value.ToString("yyyy-MM-dd");
                        Paid = Reciept.FinalTotalAmount.Value.ToString();
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
                    dbContext.EmployeesIncreases.Where(z => z.IncreaseDate.Value.ToString().Contains(date)&&z.IsDeleted==false && z.EmployeeId == x.Id).Sum(c => c.IncreaseValue) -
                    dbContext.EmployeesDiscounts.Where(y => y.DiscountDate.Value.ToString().Contains(date) && y.IsDeleted == false && y.EmployeeId == x.Id).Sum(d => d.DiscountValue)).ToString(),
                    Reciept = RecieptCount.ToString(),
                    Date = Date,
                    Paid = Paid,
                    StudyPlaceName = StudyPlace,
                    StudyYearName = StudyYear,
                }).FirstOrDefault();
                if (model.TotalDiscountCost == "")
                {
                    model.TotalDiscountCost = "0";
                }
                if (model.TotalIncreasesCost == "")
                {
                    model.TotalIncreasesCost = "0";
                }
                if(model.Reciept != "0")
                {
                    model.FinalTotalCost = model.Paid;
                }
                else
                {
                    model.FinalTotalCost = ((totalCostCount * float.Parse(model.WorkDayCost)) -
                   float.Parse(model.TotalDiscountCost) +
                   float.Parse(model.TotalIncreasesCost)).ToString();
                }
               

                return model;
            }
        }

        public ResultDto<Models.EmployeesReceipt> Create(string date, string month, Guid EmployeeId, Guid StudyYearId, Guid StudyPlaceId, float Total, float TotalDiscountCost, float TotalIncreasesCost, float Final, Guid UserId)
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

                var employeeClasses = employeesServices.Get(EmployeeId);
                var Expense = new Models.Expens
                {
                    Id = Guid.NewGuid(),
                    StudyYearId = StudyYearId,
                    StudyPlaceId = StudyPlaceId,
                    ExpenseTypeId = Guid.Parse("20A6A59E-088E-4E6A-AA27-8A15F051B1DE"),
                    ExpenseValue = Final.ToString(),
                    
                    ExpenseDate = DateTime.Parse(date).Date,
                    Notes = "راتب الموظف " + employeeClasses.Name,
                    EmployeeId = EmployeeId,
                    EmployeesReceiptId = model.Id
                };
                expensesServices.Create(Expense, UserId);

                result.Result = model;
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.EmployeesReceipt> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.EmployeesReceipt>();
                var Oldmodel = dbContext.EmployeesReceipts.Where(x => x.Id==Id&& x.IsDeleted == false ).FirstOrDefault();
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
        public ResultDto<Models.EmployeesReceipt> DeleteCollect(string Date, Guid Id, Guid UserId)
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
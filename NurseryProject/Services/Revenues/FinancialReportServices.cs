using NurseryProject.Dtos.Revenues;
using NurseryProject.Models;
using NurseryProject.Services.EmployeesReceipt;
using NurseryProject.Services.Expenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Revenues
{
    public class FinancialReportServices
    {

        public FinancialReportDto GetFinancial(string date)
        {
            using (var dbContext = new almohandes_DbEntities())
            {

                FinancialReportDto list = new FinancialReportDto();
                EmployeesReceiptServices employeesReceiptServices = new EmployeesReceiptServices();
                ExpensesServices expensesServices = new ExpensesServices();
                var Month = DateTime.Parse(date);
                var date1 = DateTime.Parse(date).ToString("yyyy-MM");
                var employeesModel = employeesReceiptServices.GetAllEmployeesReceipts().Where(x => x.Month == date1).ToList();

                list.Salaries = employeesModel.Select(x => (float)x.FinalTotalCost).DefaultIfEmpty(0).Sum();


                var Expences = dbContext.Expenses.Where(x => x.IsDeleted == false && x.ExpenseDate.Value.Month== Month.Month&& x.ExpenseDate.Value.Year == Month.Year);
                list.Expences = Expences.Select(x => x.Total).DefaultIfEmpty(0).Sum();
                if (list.Expences==null)
                    list.Expences = 0;

                if (list.Income == null)
                    list.Income = 0;

                var Income = dbContext.Revenues.Where(x => x.IsDeleted == false && x.RevenueDate.Value.Month == Month.Month && x.RevenueDate.Value.Year == Month.Year);
                list.Income = Income.Select(x => x.RevenueValue).DefaultIfEmpty(0).Sum();

                var safy = list.Income - list.Expences - list.Salaries;
                list.Safy = safy;
                return list;
            }
        }


    }
}
using NurseryProject.Dtos.EmployeeReports;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeeReports
{
    public class EmployeeReportsServices
    {
        public List<EmployeeReportsDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeReports.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin)).OrderBy(x => x.CreatedOn)
                    .Select(x => new EmployeeReportsDto
                    {
                        Id = x.Id,
                        EmployeeReportTypeId = x.EmployeeReportTypeId != null ? x.EmployeeReportTypeId : Guid.Empty,
                        EmployeeReportTypeName = x.EmployeeReportTypeId != null ? x.EmployeeReportType.Name : "",
                        EmployeeId = x.EmployeeId != null ? x.EmployeeId : Guid.Empty,
                        EmployeeName = x.EmployeeId != null ? x.Employee.Name : "",
                        EmployeeReportItems = x.EmployeeReportItems.Where(y => y.IsDeleted == false)
                        .Select(y => new EmployeeReportItemsDto
                        {
                            Id = y.Id,
                            EmployeeReportId = x.Id,
                            EmployeeReportToolId = y.EmployeeToolId != null ? (Guid)y.EmployeeToolId : Guid.Empty,
                            Name = y.EmployeeToolId != null ? y.EmployeeReportTool.Name : "",
                            Value = y.ToolValue
                        }).ToList(),
                        Notes = x.Notes
                    }).ToList();
                return model;
            }
        }
        public EmployeeReportsDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeReports.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn)
                    .Select(x => new EmployeeReportsDto
                    {
                        Id = x.Id,
                        EmployeeReportTypeId = x.EmployeeReportTypeId != null ? x.EmployeeReportTypeId : Guid.Empty,
                        EmployeeReportTypeName = x.EmployeeReportTypeId != null ? x.EmployeeReportType.Name : "",
                        EmployeeId = x.EmployeeId != null ? x.EmployeeId : Guid.Empty,
                        EmployeeName = x.EmployeeId != null ? x.Employee.Name : "",
                        EmployeeReportItems = x.EmployeeReportItems.Where(y => y.IsDeleted == false)
                        .Select(y => new EmployeeReportItemsDto
                        {
                            Id = y.Id,
                            EmployeeReportId = x.Id,
                            EmployeeReportToolId = y.EmployeeToolId != null ? (Guid)y.EmployeeToolId : Guid.Empty,
                            Name = y.EmployeeToolId != null ? y.EmployeeReportTool.Name : "",
                            Value = y.ToolValue
                        }).ToList(),
                        Notes = x.Notes
                    }).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<EmployeeReportsDto> Create(EmployeeReportsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeReportsDto>();
                var Oldmodel = dbContext.EmployeeReports.Where(x => x.ReportDateFrom == model.ReportDateFrom && x.ReportDateTo == model.ReportDateTo && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = " التقرير موجود بالفعل";
                    return result;
                }
                var newModel = new EmployeeReport();

                newModel.Id = Guid.NewGuid();
                newModel.EmployeeReportTypeId = model.EmployeeReportTypeId;
                newModel.EmployeeId = model.EmployeeId;
                newModel.ReportDateFrom = model.ReportDateFrom;
                newModel.ReportDateTo = model.ReportDateTo;
                newModel.Notes = model.Notes;
                newModel.CreatedOn = DateTime.UtcNow;
                newModel.CreatedBy = UserId;
                newModel.IsDeleted = false;

                dbContext.EmployeeReports.Add(newModel);

                foreach (var item in model.EmployeeReportItems)
                {
                    var newModel2 = new EmployeeReportItem();

                    newModel2.Id = Guid.NewGuid();
                    newModel2.EmployeeReportId = newModel.Id;
                    newModel2.EmployeeToolId = item.EmployeeReportToolId;

                    newModel2.ToolValue = item.Value;
                    newModel2.Notes = item.Notes;
                    newModel2.CreatedOn = DateTime.UtcNow;
                    newModel2.CreatedBy = UserId;
                    newModel2.IsDeleted = false;

                    dbContext.EmployeeReportItems.Add(newModel2);
                }


                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeReportsDto> Edit(EmployeeReportsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeReportsDto>();
                var Oldmodel = dbContext.EmployeeReports.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = " التقرير غير موجود ";
                    return result;
                }

                Oldmodel.EmployeeReportTypeId = model.EmployeeReportTypeId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.ReportDateFrom = model.ReportDateFrom;
                Oldmodel.ReportDateTo = model.ReportDateTo;
                Oldmodel.Notes = model.Notes;


                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;

                var oldItems = dbContext.EmployeeReportItems.Where(x => !x.IsDeleted && x.EmployeeReportId == model.Id).ToList();
                foreach (var item in oldItems)
                {
                    item.DeletedOn = DateTime.UtcNow;
                    item.DeletedBy = UserId;
                    item.IsDeleted = true;
                }
                foreach (var item in model.EmployeeReportItems)
                {
                    var newModel2 = new EmployeeReportItem();

                    newModel2.Id = Guid.NewGuid();
                    newModel2.EmployeeReportId = Oldmodel.Id;
                    newModel2.EmployeeToolId = item.EmployeeReportToolId;

                    newModel2.ToolValue = item.Value;
                    newModel2.Notes = item.Notes;
                    newModel2.CreatedOn = DateTime.UtcNow;
                    newModel2.CreatedBy = UserId;
                    newModel2.IsDeleted = false;

                    dbContext.EmployeeReportItems.Add(newModel2);
                }

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeReportsDto> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeReportsDto>();
                var Oldmodel = dbContext.EmployeeReports.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = " التقرير غير موجود ";
                    return result;
                }

                Oldmodel.IsDeleted = true;
                Oldmodel.DeletedOn = DateTime.UtcNow;
                Oldmodel.DeletedBy = UserId;

                var oldItems = dbContext.EmployeeReportItems.Where(x => !x.IsDeleted && x.EmployeeReportId == Oldmodel.Id).ToList();
                foreach (var item in oldItems)
                {
                    item.DeletedOn = DateTime.UtcNow;
                    item.DeletedBy = UserId;
                    item.IsDeleted = true;
                }

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حذف البيانات بنجاح";
                return result;
            }
        }
    }
}
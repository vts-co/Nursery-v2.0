using NurseryProject.Dtos.StudentReports;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentReports
{
    public class StudentReportsServices
    {
        public List<StudentReportsDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentReports.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin)).OrderBy(x => x.CreatedOn)
                    .Select(x => new StudentReportsDto
                    {
                        Id = x.Id,
                        StudentReportTypeId = x.StudentReportTypeId != null ? x.StudentReportTypeId : Guid.Empty,
                        StudentReportTypeName = x.StudentReportTypeId != null ? x.StudentReportType.Name : "",
                        StudentId = x.StudentId != null ? x.StudentId : Guid.Empty,
                        StudentName = x.StudentId != null ? x.Student.Name : "",
                        StudentReportItems = x.StudentReportItems.Where(y => y.IsDeleted == false)
                        .Select(y => new StudentReportItemsDto
                        {
                            Id = y.Id,
                            StudentReportId = x.Id,
                            StudentReportToolId = y.StudentToolId != null ? (Guid)y.StudentToolId : Guid.Empty,
                            Name = y.StudentToolId != null ? y.StudentReportTool.Name : "",
                            Value = y.ToolValue
                        }).ToList(),
                        Notes = x.Notes
                    }).ToList();
                return model;
            }
        }
        public StudentReportsDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentReports.Where(x => x.IsDeleted == false && x.Id==Id).OrderBy(x => x.CreatedOn)
                    .Select(x => new StudentReportsDto
                    {
                        Id = x.Id,
                        StudentReportTypeId = x.StudentReportTypeId != null ? x.StudentReportTypeId : Guid.Empty,
                        StudentReportTypeName = x.StudentReportTypeId != null ? x.StudentReportType.Name : "",
                        StudentId = x.StudentId != null ? x.StudentId : Guid.Empty,
                        StudentName = x.StudentId != null ? x.Student.Name : "",
                        StudentReportItems = x.StudentReportItems.Where(y => y.IsDeleted == false)
                        .Select(y => new StudentReportItemsDto
                        {
                            Id = y.Id,
                            StudentReportId = x.Id,
                            StudentReportToolId = y.StudentToolId != null ? (Guid)y.StudentToolId : Guid.Empty,
                            Name = y.StudentToolId != null ? y.StudentReportTool.Name : "",
                            Value = y.ToolValue
                        }).ToList(),
                        Notes = x.Notes
                    }).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<StudentReportsDto> Create(StudentReportsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentReportsDto>();
                var Oldmodel = dbContext.StudentReports.Where(x =>x.ReportDateFrom==model.ReportDateFrom&&x.ReportDateTo==model.ReportDateTo && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = " التقرير موجود بالفعل";
                    return result;
                }
                var newModel = new StudentReport();

                newModel.Id = Guid.NewGuid();
                newModel.StudentReportTypeId = model.StudentReportTypeId;
                newModel.StudentId = model.StudentId;
                newModel.ReportDateFrom = model.ReportDateFrom;
                newModel.ReportDateTo = model.ReportDateTo;
                newModel.Notes = model.Notes;
                newModel.CreatedOn = DateTime.UtcNow;
                newModel.CreatedBy = UserId;
                newModel.IsDeleted = false;

                dbContext.StudentReports.Add(newModel);

                foreach (var item in model.StudentReportItems)
                {
                    var newModel2 = new StudentReportItem();

                    newModel2.Id = Guid.NewGuid();
                    newModel2.StudentReportId = newModel.Id;
                    newModel2.StudentToolId = item.StudentReportToolId;
                    
                    newModel2.ToolValue = item.Value;
                    newModel2.Notes = item.Notes;
                    newModel2.CreatedOn = DateTime.UtcNow;
                    newModel2.CreatedBy = UserId;
                    newModel2.IsDeleted = false;

                    dbContext.StudentReportItems.Add(newModel2);
                }
               

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentReportsDto> Edit(StudentReportsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentReportsDto>();
                var Oldmodel = dbContext.StudentReports.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = " التقرير غير موجود ";
                    return result;
                }

                Oldmodel.StudentReportTypeId = model.StudentReportTypeId;
                Oldmodel.StudentId = model.StudentId;
                Oldmodel.ReportDateFrom = model.ReportDateFrom;
                Oldmodel.ReportDateTo = model.ReportDateTo;
                Oldmodel.Notes = model.Notes;
            

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;

                var oldItems =dbContext.StudentReportItems.Where(x=>!x.IsDeleted&&x.StudentReportId== model.Id).ToList();
                foreach (var item in oldItems)
                {
                    item.DeletedOn = DateTime.UtcNow;
                    item.DeletedBy = UserId;
                    item.IsDeleted = true;
                }
                foreach (var item in model.StudentReportItems)
                {
                    var newModel2 = new StudentReportItem();

                    newModel2.Id = Guid.NewGuid();
                    newModel2.StudentReportId = Oldmodel.Id;
                    newModel2.StudentToolId = item.StudentReportToolId;

                    newModel2.ToolValue = item.Value;
                    newModel2.Notes = item.Notes;
                    newModel2.CreatedOn = DateTime.UtcNow;
                    newModel2.CreatedBy = UserId;
                    newModel2.IsDeleted = false;

                    dbContext.StudentReportItems.Add(newModel2);
                }

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentReportsDto> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentReportsDto>();
                var Oldmodel = dbContext.StudentReports.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = " التقرير غير موجود ";
                    return result;
                }

                Oldmodel.IsDeleted = true;
                Oldmodel.DeletedOn = DateTime.UtcNow;
                Oldmodel.DeletedBy = UserId;

                var oldItems = dbContext.StudentReportItems.Where(x => !x.IsDeleted && x.StudentReportId == Oldmodel.Id).ToList();
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
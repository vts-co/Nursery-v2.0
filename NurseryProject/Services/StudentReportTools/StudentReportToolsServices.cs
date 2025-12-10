using NurseryProject.Dtos.EmployeeReportTools;
using NurseryProject.Dtos.StudentReportTools;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentReportTools
{
    public class StudentReportToolsServices
    {
        public List<StudentReportToolsDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentReportTools.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin)).OrderBy(x => x.CreatedOn)
                    .Select(x => new StudentReportToolsDto
                    {
                        Id = x.Id,
                        StudentReportTypeId = x.StudentReportTypeId != null ? x.StudentReportTypeId : Guid.Empty,
                        StudentReportTypeName = x.StudentReportTypeId != null ? x.StudentReportType.Name : "",
                        Name = x.Name,
                        Notes = x.Notes
                    }).ToList();
                return model;
            }
        }
        public List<StudentReportToolsDto> GetAllByStudentReportTypeId(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentReportTools.Where(x => x.IsDeleted == false &&x.StudentReportTypeId==Id).OrderBy(x => x.CreatedOn)
                    .Select(x => new StudentReportToolsDto
                    {
                        Id = x.Id,
                        StudentReportTypeId = x.StudentReportTypeId != null ? x.StudentReportTypeId : Guid.Empty,
                        StudentReportTypeName = x.StudentReportTypeId != null ? x.StudentReportType.Name : "",
                        Name = x.Name,
                        Notes = x.Notes
                    }).ToList();
                return model;
            }
        }

        public StudentReportToolsDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentReportTools.Where(x => x.IsDeleted == false&&x.Id==Id).OrderBy(x => x.CreatedOn)
                    .Select(x => new StudentReportToolsDto
                    {
                        Id = x.Id,
                        StudentReportTypeId = x.StudentReportTypeId != null ? x.StudentReportTypeId : Guid.Empty,
                        StudentReportTypeName = x.StudentReportTypeId != null ? x.StudentReportType.Name : "",
                        Name = x.Name,
                        Notes = x.Notes
                    }).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<StudentReportToolsDto> Create(StudentReportToolsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentReportToolsDto>();
                var Oldmodel = dbContext.StudentReportTools.Where(x => x.Id == model.Id && x.Name == model.Name && x.StudentReportTypeId==model.StudentReportTypeId && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "محتوي التقرير موجود بالفعل";
                    return result;
                }
                var newModel = new StudentReportTool();

                newModel.Id = Guid.NewGuid();
                newModel.StudentReportTypeId = model.StudentReportTypeId;
                newModel.Name = model.Name;
                newModel.Notes = model.Notes;
                newModel.CreatedOn = DateTime.UtcNow;
                newModel.CreatedBy = UserId;
                newModel.IsDeleted = false;

                dbContext.StudentReportTools.Add(newModel);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentReportToolsDto> Edit(StudentReportToolsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentReportToolsDto>();
                var Oldmodel = dbContext.StudentReportTools.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "محتوي التقرير غير موجود ";
                    return result;
                }
                
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.StudentReportTypeId = model.StudentReportTypeId;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentReportToolsDto> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentReportToolsDto>();
                var Oldmodel = dbContext.StudentReportTools.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "محتوي التقرير غير موجود ";
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
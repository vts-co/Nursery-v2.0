using NurseryProject.Dtos.EmployeeReportTools;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.EmployeeReportTools
{
    public class EmployeeReportToolsServices
    {
        public List<EmployeeReportToolsDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeReportTools.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin)).OrderBy(x => x.CreatedOn)
                    .Select(x => new EmployeeReportToolsDto
                    {
                        Id = x.Id,
                        EmployeeReportTypeId = x.EmployeeReportTypeId != null ? x.EmployeeReportTypeId : Guid.Empty,
                        EmployeeReportTypeName = x.EmployeeReportTypeId != null ? x.EmployeeReportType.Name : "",
                        Name = x.Name,
                        Notes = x.Notes
                    }).ToList();
                return model;
            }
        }
        public List<EmployeeReportToolsDto> GetAllByEmployeeReportTypeId(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeReportTools.Where(x => x.IsDeleted == false && x.EmployeeReportTypeId==Id).OrderBy(x => x.CreatedOn)
                    .Select(x => new EmployeeReportToolsDto
                    {
                        Id = x.Id,
                        EmployeeReportTypeId = x.EmployeeReportTypeId != null ? x.EmployeeReportTypeId : Guid.Empty,
                        EmployeeReportTypeName = x.EmployeeReportTypeId != null ? x.EmployeeReportType.Name : "",
                        Name = x.Name,
                        Notes = x.Notes
                    }).ToList();
                return model;
            }
        }
        public EmployeeReportToolsDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.EmployeeReportTools.Where(x => x.IsDeleted == false&&x.Id==Id).OrderBy(x => x.CreatedOn)
                    .Select(x => new EmployeeReportToolsDto
                    {
                        Id = x.Id,
                        EmployeeReportTypeId = x.EmployeeReportTypeId != null ? x.EmployeeReportTypeId : Guid.Empty,
                        EmployeeReportTypeName = x.EmployeeReportTypeId != null ? x.EmployeeReportType.Name : "",
                        Name = x.Name,
                        Notes = x.Notes
                    }).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<EmployeeReportToolsDto> Create(EmployeeReportToolsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeReportToolsDto>();
                var Oldmodel = dbContext.EmployeeReportTools.Where(x => x.Id == model.Id && x.Name == model.Name && x.EmployeeReportTypeId==model.EmployeeReportTypeId && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "محتوي التقرير موجود بالفعل";
                    return result;
                }
                var newModel = new EmployeeReportTool();

                newModel.Id = Guid.NewGuid();
                newModel.EmployeeReportTypeId = model.EmployeeReportTypeId;
                newModel.Name = model.Name;
                newModel.Notes = model.Notes;
                newModel.CreatedOn = DateTime.UtcNow;
                newModel.CreatedBy = UserId;
                newModel.IsDeleted = false;

                dbContext.EmployeeReportTools.Add(newModel);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeReportToolsDto> Edit(EmployeeReportToolsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeReportToolsDto>();
                var Oldmodel = dbContext.EmployeeReportTools.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "محتوي التقرير غير موجود ";
                    return result;
                }
                
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.EmployeeReportTypeId = model.EmployeeReportTypeId;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<EmployeeReportTool> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<EmployeeReportTool>();
                var Oldmodel = dbContext.EmployeeReportTools.Find(Id);
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
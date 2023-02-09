using NurseryProject.Dtos.BuildingSupervisors;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.BuildingSupervisors
{
    public class BuildingSupervisorsServices
    {
        public List<BuildingSupervisorsDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.BuildingSupervisors.Where(x => x.IsDeleted == false && x.Employee.IsDeleted == false && x.StudyPlace.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new BuildingSupervisorsDto
                {
                    Id = x.Id,
                    StudyPlaceId = x.StudyPlaceId != null ? x.StudyPlaceId.Value : Guid.Empty,
                    StudyPlaceName = x.StudyPlaceId != null ? x.StudyPlace.Name : "",
                    DepartmentId = x.Employee.Jop.DepartmentId != null ? x.Employee.Jop.DepartmentId.Value:Guid.Empty,
                    DepartmentName = x.Employee.Jop.DepartmentId != null ? x.Employee.Jop.Department.Name:"",
                    EmployeeId = x.EmployeeId!=null? x.EmployeeId.Value:Guid.Empty,
                    EmployeeName = x.EmployeeId != null ? x.Employee.Name:"",
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public BuildingSupervisor Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.BuildingSupervisors.Where(x => x.Id == Id && x.IsDeleted == false).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }

        public ResultDto<BuildingSupervisor> Create(BuildingSupervisor model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<BuildingSupervisor>();
                var Oldmodel = dbContext.BuildingSupervisors.Where(x => x.StudyPlaceId == model.StudyPlaceId && x.EmployeeId == model.EmployeeId && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا المشرف موجود بالفعل بالمبني";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.BuildingSupervisors.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<BuildingSupervisor> Edit(BuildingSupervisor model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<BuildingSupervisor>();
                var Oldmodel = dbContext.BuildingSupervisors.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا المشرف غير موجود بالمبني ";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.StudyPlaceId = model.StudyPlaceId;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<BuildingSupervisor> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<BuildingSupervisor>();
                var Oldmodel = dbContext.BuildingSupervisors.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا المشرف غير موجود بالمبني ";
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
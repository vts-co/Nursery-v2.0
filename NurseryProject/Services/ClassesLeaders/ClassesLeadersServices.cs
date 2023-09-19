using NurseryProject.Dtos.ClassesLeaders;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.ClassesLeaders
{
    public class ClassesLeadersServices
    {
        public List<ClassesLeadersDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.ClassesLeaders.Where(x => x.IsDeleted == false && !x.Employee.EmployeesVacations.Any(y => !y.IsDeleted && y.DateFrom <= DateTime.Now && y.DateTo <= DateTime.Now)&&(x.CreatedBy== UserId||RoleId==Role.SystemAdmin||x.EmployeeId== EmployeeId || x.Class.StudyPlace.BuildingSupervisors.Any(y=>y.IsDeleted==false&& y.EmployeeId == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new ClassesLeadersDto
                {
                    Id = x.Id,
                    StudyPlaceId = x.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.Class.StudyPlace.Name,
                    StudyTypeId = x.Class.Level.StudyTypeId.Value,
                    StudyTypeName = x.Class.Level.StudyType.Name,
                    LevelId = x.Class.LevelId.Value,
                    LevelName = x.Class.Level.Name,
                    ClassId = x.ClassId.Value,
                    ClassName = x.Class.Name,
                    DepartmentId=x.Employee.Jop.DepartmentId.Value,
                    DepartmentName=x.Employee.Jop.Department.Name,
                    EmployeeId = x.EmployeeId.Value,
                    EmployeeName = x.Employee.Name,
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public ClassesLeader Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.ClassesLeaders.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<ClassesLeader> Create(ClassesLeader model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ClassesLeader>();

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.ClassesLeaders.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<ClassesLeader> Edit(ClassesLeader model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ClassesLeader>();
                var Oldmodel = dbContext.ClassesLeaders.Find(model.Id);

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.ClassId = model.ClassId;
                Oldmodel.EmployeeId = model.EmployeeId;
                Oldmodel.Notes = model.Notes;
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<ClassesLeader> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ClassesLeader>();
                var Oldmodel = dbContext.ClassesLeaders.Find(Id);
                
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
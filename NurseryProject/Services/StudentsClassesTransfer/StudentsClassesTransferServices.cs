using NurseryProject.Dtos.StudentsClassesTransfer;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentsClassesTransfer
{
    public class StudentsClassesTransferServices
    {
        public List<StudentsClassesTransferDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClassesTransfers.Where(x => x.IsDeleted == false && x.StudentsClass.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId) || x.Class1.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class1.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId) || x.Class.StudyPlace.BuildingSupervisors.Any(k => k.IsDeleted == false && k.EmployeeId == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassesTransferDto
                {
                    Id = x.Id,
                    StudentClassId = x.StudentClassId.Value,

                    StudyYearId = x.StudentsClass.StudyYearId.Value,
                    StudyYearName = x.StudentsClass.StudyYear.Name,

                    StudyPlaceId = x.StudentsClass.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.StudentsClass.Class.StudyPlace.Name,

                    StudyTypeId = x.StudentsClass.Class.Level.StudyTypeId.Value,
                    StudyTypeName = x.StudentsClass.Class.Level.StudyType.Name,

                    LevelId = x.StudentsClass.Class.LevelId.Value,
                    LevelName = x.StudentsClass.Class.Level.Name,

                    ClassFromId = x.Class.Id,
                    ClassFromName = x.Class.Name,

                    ClassToId = x.Class1.Id,
                    ClassToName = x.Class1.Name,

                    Code=x.StudentsClass.Student.Code,
                    StudentId = x.StudentsClass.Student.Id,
                    StudentName = x.StudentsClass.Student.Name,

                    Date = x.Date.ToString(),

                    Notes = x.Notes

                }).ToList();
                
                return model;
            }
        }
        public StudentsClassesTransferDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsClassesTransfers.Where(x => x.IsDeleted == false && x.Id == Id && x.StudentsClass.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new StudentsClassesTransferDto
                {
                    Id = x.Id,
                    StudentClassId = x.StudentClassId.Value,

                    StudyYearId = x.StudentsClass.StudyYearId.Value,
                    StudyYearName = x.StudentsClass.StudyYear.Name,

                    StudyPlaceId = x.StudentsClass.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.StudentsClass.Class.StudyPlace.Name,

                    StudyTypeId = x.StudentsClass.Class.Level.StudyTypeId.Value,
                    StudyTypeName = x.StudentsClass.Class.Level.StudyType.Name,

                    LevelId = x.StudentsClass.Class.LevelId.Value,
                    LevelName = x.StudentsClass.Class.Level.Name,

                    ClassFromId = x.Class.Id,
                    ClassFromName = x.Class.Name,

                    ClassToId = x.Class1.Id,
                    ClassToName = x.Class1.Name,

                    Code = x.StudentsClass.Student.Code,
                    StudentId = x.StudentsClass.Student.Id,
                    StudentName = x.StudentsClass.Student.Name,

                    Date = x.Date.ToString(),

                    Notes = x.Notes
                }).FirstOrDefault();
                
                return model;
            }
        }
        public ResultDto<StudentsClassesTransferDto> Create(StudentsClassesTransferDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassesTransferDto>();
               
                var studentsClass = new Models.StudentsClassesTransfer()
                {
                    Id = model.Id,
                    StudentClassId = model.StudentClassId,
                    ClassFromId = model.ClassFromId,
                    ClassToId = model.ClassToId,
                    Date = DateTime.Parse(model.Date),
                    Notes = model.Notes,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = UserId,
                    IsDeleted = false,
                };
                dbContext.StudentsClassesTransfers.Add(studentsClass);
                dbContext.SaveChanges();
                
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentsClassesTransferDto> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentsClassesTransferDto>();
                var Oldmodel = dbContext.StudentsClassesTransfers.Find(Id);
                
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
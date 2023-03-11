using NurseryProject.Controllers;
using NurseryProject.Dtos.StudentsAttendance;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentsAttendance
{
    public class StudentsAttendanceServices
    {
        public List<StudentsAttendanceDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var date1 = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId) || x.Class.StudyPlace.BuildingSupervisors.Any(k => k.IsDeleted == false && k.EmployeeId == EmployeeId))).OrderBy(x => x.CreatedOn)
                    .Select(x => new { x.Id, x.Date, Code = x.Student.Code, x.ClassId, x.Class.Name, Plase = x.Class.StudyPlace.Name, Type = x.Class.Level.StudyType.Name, Level = x.Class.Level.Name, Class = x.StudyClass.Name, Year = x.StudyClass.StudyYear.Name }).ToList();
                var model1 = date1.Count();
                var mod = new List<StudentsAttendanceDto>();

                if (date1.Count() > 0)
                {
                    var date = date1[0].Date;
                    var class1 = date1[0].ClassId;

                    for (int i = 1; i < model1; i++)
                    {
                        if (date == date1[i].Date && class1 == date1[i].ClassId)
                        {
                            date1.Remove(date1[i]);
                            i--;
                            model1 -= 1;
                        }
                        else
                        {
                            date = date1[i].Date;
                            class1 = date1[i].ClassId;
                        }
                    }
                    foreach (var item in date1)
                    {
                        var model = dbContext.StudentsClasses.Where(x => x.IsDeleted == false && x.ClassId == item.ClassId).ToList();
                        var model2 = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.Date == item.Date && x.ClassId == item.ClassId && x.IsAttend == true).ToList();

                        mod.Add(new StudentsAttendanceDto { Id = item.Id, Code = item.Code, StudyPlaceName = item.Plase, StudyTypeName = item.Type, LevelName = item.Level, StudyYearName = item.Year, StudyClassName = item.Class, NumAllAttend = model.Count().ToString(), NumAttend = model2.Count().ToString(), ClassName = item.Name, Date = item.Date.Value.ToString("yyyy-MM-dd") });
                    };
                }

                return mod;
            }
        }
        public List<StudentsAttendanceDto> GetAllByStudent(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var date1 = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId))).OrderBy(x => x.CreatedOn).ToList();
                var model1 = date1.Count();
                var mod = new List<StudentsAttendanceDto>();

                if (date1.Count() > 0)
                {
                    var class1 = date1[0].ClassId;
                    var student = date1[0].StudentId;


                    for (int i = 1; i < model1; i++)
                    {
                        if (student == date1[i].StudentId && class1 == date1[i].ClassId)
                        {
                            date1.Remove(date1[i]);
                            i--;
                            model1 -= 1;
                        }
                        else
                        {
                            student = date1[i].StudentId;
                            class1 = date1[i].ClassId;
                        }
                    }
                    foreach (var item in date1)
                    {
                        var model = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.StudentId == item.StudentId && x.ClassId == item.ClassId && x.IsAttend == false).ToList();
                        var model2 = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.StudentId == item.StudentId && x.ClassId == item.ClassId && x.IsAttend == true).ToList();

                        mod.Add(new StudentsAttendanceDto { Id = item.Id,StudentName=item.Student.Name, Code = item.Student.Code, StudyPlaceName = item.Class.StudyPlace.Name, StudyTypeName = item.Class.Level.StudyType.Name, LevelName = item.Class.Level.Name, StudyYearName = item.StudyClass.StudyYear.Name,StudyYearId=item.StudyClass.StudyYear.Id, StudyClassName = item.StudyClass.Name,StudyClassId=item.StudyClass.Id, NumAllAttend = model.Count().ToString(), NumAttend = model2.Count().ToString(), ClassName = item.Class.Name, Date = item.Date.Value.ToString("yyyy-MM-dd"),
                        Days= dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.StudentId == item.StudentId && x.ClassId == item.ClassId).Select(y=>new StudentsAttendanceDaysDto {Date=y.Date.Value.ToString(),IsAttend=y.IsAttend.Value }).ToList(),

                        });
                    };
                }

                return mod;
            }
        }
        public Models.StudentsAttendance Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public List<StudentsAttendanceDto> GetAllAttendance(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new StudentsAttendanceDto
                {
                    Id = x.Id,
                    StudentId = x.Student.Id,
                    StudentName = x.Student.Name,
                    Code = x.Student.Code,
                    StudyClassId = x.StudyClass.Id,
                    StudyClassName = x.StudyClass.Name,
                    StudyPlaceId = x.Class.StudyPlace.Id,
                    StudyPlaceName = x.Class.StudyPlace.Name,
                    StudyTypeId = x.Class.Level.StudyType.Id,
                    StudyTypeName = x.Class.Level.StudyType.Name,
                    StudyYearName = x.StudyClass.StudyYear.Name,
                    StudyYearId = x.StudyClass.StudyYear.Id,
                    ClassId = x.Class.Id,
                    ClassName = x.Class.Name,
                    LevelId = x.Class.Level.Id,
                    LevelName = x.Class.Level.Name,

                    Date = x.Date.Value.ToString(),
                    IsAttend = x.IsAttend.Value,
                }
                ).ToList();
               
                return model;
            }
        }
        public List<StudentsAttendanceDto> GetAllNoAttendance(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.IsAttend == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new StudentsAttendanceDto
                {
                    Id = x.Id,
                    StudentId = x.Student.Id,
                    StudentName = x.Student.Name,
                    Code = x.Student.Code,
                    StudyClassId = x.StudyClass.Id,
                    StudyClassName = x.StudyClass.Name,
                    StudyPlaceId = x.Class.StudyPlace.Id,
                    StudyPlaceName = x.Class.StudyPlace.Name,
                    StudyTypeId = x.Class.Level.StudyType.Id,
                    StudyTypeName = x.Class.Level.StudyType.Name,
                    StudyYearName = x.StudyClass.StudyYear.Name,
                    StudyYearId = x.StudyClass.StudyYear.Id,
                    ClassId = x.Class.Id,
                    ClassName = x.Class.Name,
                    LevelId = x.Class.Level.Id,
                    LevelName = x.Class.Level.Name,

                    Date = x.Date.Value.ToString(),
                    IsAttend = x.IsAttend.Value
                }
                ).ToList();
                return model;
            }
        }
        public int Get(DateTime Date, Guid ClassId, Guid StudyYearId, Guid StudyClassId, Guid StudentId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.StudentId == StudentId && x.ClassId == ClassId && x.StudyClassId == StudyClassId && x.StudyClass.StudyYearId == StudyYearId && x.Date == Date && x.IsAttend == true).OrderBy(x => x.CreatedOn).ToList().Count();
                return model;
            }
        }
        public ResultDto<Models.StudentsAttendance> Create(Models.StudentsAttendance model, List<Attend> IsAttend, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.StudentsAttendance>();

                var test = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.ClassId == model.ClassId && x.StudyClassId == model.StudyClassId && x.Date == model.Date).ToList();
                if (test.Count() > 0)
                {
                    result.IsSuccess = false;
                    result.Message = "تم اخذ الغياب لهذا اليوم";
                    return result;
                }
                foreach (var item in IsAttend)
                {
                    Models.StudentsAttendance model2 = new Models.StudentsAttendance();
                    var id = Guid.NewGuid();
                    model2.Id = id;
                    model2.ClassId = model.ClassId;
                    model2.StudyClassId = model.StudyClassId;
                    model2.Date = model.Date;

                    model2.CreatedOn = DateTime.UtcNow;
                    model2.CreatedBy = UserId;
                    model2.IsDeleted = false;
                    model2.StudentId = item.Id;


                    if (item.Att == "on")
                    {
                        model2.IsAttend = true;
                    }
                    else
                    {
                        model2.IsAttend = false;
                    }

                    dbContext.StudentsAttendances.Add(model2);
                    dbContext.SaveChanges();

                }

                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Models.StudentsAttendance> Edit(Models.StudentsAttendance model, List<Attend> IsAttend, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.StudentsAttendance>();

                var test = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.ClassId == model.ClassId && x.StudyClassId == model.StudyClassId && x.Date == model.Date).ToList();
                if (test.Count() > 0)
                {
                    foreach (var item in test)
                    {
                        item.IsDeleted = true;
                        item.DeletedOn = DateTime.UtcNow;
                        item.DeletedBy = UserId;
                        dbContext.SaveChanges();
                    }
                }
                foreach (var item in IsAttend)
                {
                    Models.StudentsAttendance model2 = new Models.StudentsAttendance();
                    var id = Guid.NewGuid();
                    model2.Id = id;
                    model2.ClassId = model.ClassId;
                    model2.StudyClassId = model.StudyClassId;
                    model2.Date = model.Date;

                    model2.CreatedOn = DateTime.UtcNow;
                    model2.CreatedBy = UserId;
                    model2.IsDeleted = false;
                    model2.StudentId = item.Id;


                    if (item.Att == "on")
                    {
                        model2.IsAttend = true;
                    }
                    else
                    {
                        model2.IsAttend = false;
                    }

                    dbContext.StudentsAttendances.Add(model2);
                    dbContext.SaveChanges();

                }
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }

        }
        public ResultDto<Models.StudentsAttendance> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Models.StudentsAttendance>();
                var Oldmodel = dbContext.StudentsAttendances.Find(Id);

                var model = dbContext.StudentsAttendances.Where(x => x.IsDeleted == false && x.ClassId == Oldmodel.ClassId && x.Date == Oldmodel.Date).ToList();
                foreach (var item in model)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    item.DeletedBy = UserId;
                    dbContext.SaveChanges();
                }

                result.IsSuccess = true;
                result.Message = "تم حذف البيانات بنجاح";
                return result;
            }
        }
    }
}
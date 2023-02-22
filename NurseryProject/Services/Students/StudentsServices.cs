using NurseryProject.Dtos.EmployeeClasses;
using NurseryProject.Dtos.Students;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Students
{
    public class StudentsServices
    {
        public List<StudentsDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Students.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new StudentsDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Phone = x.Phone,
                    Address = x.Address,
                    Image = x.Image,
                    BirthDate = x.BirthDate.ToString(),
                    GenderId = x.GenderId.Value,
                    GenderName = x.GenderId == 1 ? "ذكر" : "انثي",
                    MotherName = x.MotherName,
                    RegistrationTypeId = x.RegistrationTypeId.Value,
                    RegistrationTypeName = x.RegistrationType.Name,
                    JoiningDate = x.JoiningDate.Value.ToString(),
                    CityId = x.DestrictId != null ? x.Destrict.CityId.Value:Guid.Empty,
                    CityName = x.DestrictId != null ? x.Destrict.City.Name:"",
                    DestrictId =x.DestrictId!=null? x.DestrictId.Value:Guid.Empty,
                    DestrictName = x.DestrictId != null ? x.Destrict.Name:"",
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public List<StudentsReportDto> GetAllReport(Guid StudyYearId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Students.Where(x => x.IsDeleted == false && x.StudentsClasses.Any(y => y.StudyYearId == StudyYearId && y.IsDeleted == false)).OrderBy(x => x.CreatedOn).Select(x => new StudentsReportDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Phone = x.Phone,
                    Address = x.Address,
                    CountOfLatecomers = "",
                    CountOfNormal = "",
                    CountOfPart = "",
                    CountOfPaidNoTime="",
                    AttendanceNum = x.StudentsAttendances.Where(c => c.IsDeleted == false && c.IsAttend == true).ToList().Count().ToString(),
                    NoAttendanceNum = x.StudentsAttendances.Where(c => c.IsDeleted == false && c.IsAttend == false).ToList().Count().ToString(),
                    CountOfTransferClasses = dbContext.StudentsClassesTransfers.Where(i => i.IsDeleted == false && i.StudentsClass.IsDeleted == false && i.StudentsClass.StudentId == x.Id).ToList().Count().ToString(),
                    ExamsRate = ((x.StudentsExamDegrees.Where(a => a.IsDeleted == false).Sum(a => a.Degree)) / (x.StudentsExamDegrees.Where(a => a.IsDeleted == false).Sum(a => a.ClassExam.Exam.TotalDegree)) * 100).ToString(),
                    Employees = x.StudentsClasses.Where(b => b.IsDeleted == false).FirstOrDefault().Class.EmployeeClasses.Where(e => e.IsDeleted == false).Select(e => new EmployeeClassesDto
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = e.Employee.Name,
                        EmployeePhone = e.Employee.Phone
                    }).ToList()
                }).ToList();
                foreach (var item in model)
                {
                    var count1 = 0;
                    var count2 = 0;
                    var count3 = 0;
                    var count4 = 0;

                    var student = dbContext.Students.Where(x => x.IsDeleted == false && x.Id == item.Id && x.StudentsClasses.Any(y => y.StudyYearId == StudyYearId && y.IsDeleted == false)).FirstOrDefault();
                    var StudentsClasses = student.StudentsClasses.Where(z => z.IsDeleted == false).ToList();
                    var SubscriptionMethods = StudentsClasses[0].SubscriptionMethods.Where(u => u.IsPaid == false).ToList();
                    var SubscriptionMethods2 = StudentsClasses[0].SubscriptionMethods.Where(u => u.IsPaid == true).ToList();

                    foreach (var item2 in SubscriptionMethods)
                    {
                        if (item2.Date.Value.Date.AddDays(15) < DateTime.Now.Date)
                        {
                            count1 += 1;
                        }
                    }
                    foreach (var item3 in SubscriptionMethods2)
                    {
                        if (item3.Date.Value.Date.AddDays(15) >= item3.PaidDate.Value.Date)
                        {
                            count2 += 1;
                        }
                        if (float.Parse(item3.Amount) > float.Parse(item3.PaidAmount))
                        {
                            count3 += 1;
                        }
                        if (item3.Date.Value.Date.AddDays(15) <= item3.PaidDate.Value.Date)
                        {
                            count4 += 1;
                        }
                    }
                    item.CountOfLatecomers = count1.ToString();
                    item.CountOfNormal = count2.ToString();
                    item.CountOfPart = count3.ToString();
                    item.CountOfPaidNoTime = count4.ToString();
                }
                return model;
            }
        }
        public List<StudentsDto> GetAllDropDown()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Students.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new StudentsDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    OName= x.Name,
                    Name = x.Code + "|" + x.Name + "|" + x.Phone,
                    Phone = x.Phone,
                    Address = x.Address,
                    Image = x.Image,
                    BirthDate = x.BirthDate.ToString(),
                    GenderId = x.GenderId.Value,
                    GenderName = x.GenderId == 1 ? "ذكر" : "انثي",
                    MotherName = x.MotherName,
                    RegistrationTypeId = x.RegistrationTypeId.Value,
                    RegistrationTypeName = x.RegistrationType.Name,
                    JoiningDate = x.JoiningDate.Value.ToString(),
                    CityId = x.DestrictId != null ? x.Destrict.CityId.Value : Guid.Empty,
                    CityName = x.DestrictId != null ? x.Destrict.City.Name : "",
                    DestrictId = x.DestrictId != null ? x.DestrictId.Value : Guid.Empty,
                    DestrictName = x.DestrictId != null ? x.Destrict.Name : "",
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public Student Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Students.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Student> Create(Student model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Student>();
                var Oldmodel = dbContext.Students.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب موجود بالفعل";
                    return result;
                }
                var Oldmodel2 = dbContext.Students.Where(x => x.Code == model.Code && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel2 != null)
                {
                    result.Result = Oldmodel2;
                    result.IsSuccess = false;
                    result.Message = "هذا الكود موجود لم يمكن استخدامه";
                    return result;
                }
                model.RegistrationTypeId = Guid.Parse("E31AC343-47DA-4DFE-8970-E1719DEEC869");
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Students.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Student> Edit(Student model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Student>();
                var Oldmodel = dbContext.Students.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب غير موجود ";
                    return result;
                }
                var Oldmodel2 = dbContext.Students.Where(x => x.Code == model.Code && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel2 != null)
                {
                    result.Result = model;
                    result.IsSuccess = false;
                    result.Message = "هذا الكود موجود لم يمكن استخدامه";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                if (model.Code != null)
                    Oldmodel.Code = model.Code;

                Oldmodel.Name = model.Name;
                Oldmodel.Phone = model.Phone;
                Oldmodel.Address = model.Address;
                Oldmodel.BirthDate = model.BirthDate;
                Oldmodel.GenderId = model.GenderId;
                Oldmodel.MotherName = model.MotherName;
                Oldmodel.RegistrationTypeId = model.RegistrationTypeId.Value;
                Oldmodel.JoiningDate = model.JoiningDate.Value;
                Oldmodel.DestrictId = model.DestrictId.Value;
                Oldmodel.Notes = model.Notes;
                if (model.Image != null)
                {
                    Oldmodel.Image = model.Image;
                }
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Student> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Student>();
                var Oldmodel = dbContext.Students.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب غير موجود ";
                    return result;
                }
                var Oldmodel2 = dbContext.SubscriptionMethods.Where(x => x.IsDeleted == false && x.StudentsClass.IsDeleted == false && x.StudentsClass.Student.IsDeleted == false && x.StudentsClass.StudentId == Id && x.IsPaid == false).ToList();
                if (Oldmodel2.Count() > 0)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب لديه اشتراك لم يمكن حذفه ";
                    return result;
                }
                if (Oldmodel.StudentsAttendances.Any(y => y.IsDeleted == false) || Oldmodel.StudentsClasses.Any(y => y.IsDeleted == false&&y.IsCurrent==true) || Oldmodel.StudentsExamDegrees.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب لا يمكن حذفه ";
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
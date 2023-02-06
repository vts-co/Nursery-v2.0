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
                var model = dbContext.Students.Where(x => x.IsDeleted == false&&x.Destrict.IsDeleted==false&&x.RegistrationType.IsDeleted==false).OrderBy(x => x.CreatedOn).Select(x => new StudentsDto
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
                    CityId = x.Destrict.CityId.Value,
                    CityName = x.Destrict.City.Name,
                    DestrictId = x.DestrictId.Value,
                    DestrictName = x.Destrict.Name,
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public List<StudentsDto> GetAllDropDown()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Students.Where(x => x.IsDeleted == false && x.Destrict.IsDeleted == false && x.RegistrationType.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new StudentsDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Code+"|"+x.Name+"|"+x.Phone,
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
                    CityId = x.Destrict.CityId.Value,
                    CityName = x.Destrict.City.Name,
                    DestrictId = x.DestrictId.Value,
                    DestrictName = x.Destrict.Name,
                    Notes = x.Notes
                }).ToList();
                return model;
            }
        }
        public Student Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Students.Where(x => x.IsDeleted == false && x.Id==Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
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
                if(model.Image!=null)
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
                var Oldmodel2 = dbContext.SubscriptionMethods.Where(x=>x.IsDeleted==false&&x.StudentsClass.IsDeleted==false &&x.StudentsClass.Student.IsDeleted==false && x.StudentsClass.StudentId==Id &&x.IsPaid==false).ToList();
               if(Oldmodel2.Count()>0)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الطالب لديه اشتراك لم يمكن حذفه ";
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
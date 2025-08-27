using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudyYears
{
    public class StudyYearsServices
    {
        public List<StudyYear> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudyYears.Where(x => x.IsDeleted == false).OrderBy(x => x.DisplayOrder).ToList();
                return model;
            }
        }
           public List<StudyPlace> GetAllStudyPlaces()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudyPlaces.Where(x => x.IsDeleted == false).ToList();
                return model;
            }
        }

        public ResultDto<StudyYear> Create(StudyYear model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyYear>();
                var Oldmodel = dbContext.StudyYears.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "هذا العام موجود بالفعل";
                    return result;
                }
               
                if (model.IsCurrentYear == true)
                {
                    var Oldmodel2 = dbContext.StudyYears.Where(x => x.IsCurrentYear == true && x.IsDeleted == false).FirstOrDefault();
                    if (Oldmodel2 != null)
                    {
                        result.Result = Oldmodel2;
                        result.IsSuccess = false;
                        result.Message = "يوجد عام حالي اخر";
                        return result;
                    }
                }

                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.StudyYears.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudyYear> Edit(StudyYear model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyYear>();
                var Oldmodel = dbContext.StudyYears.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا العام غير موجود ";
                    return result;
                }
                if (model.IsCurrentYear == true)
                {
                    var Oldmodel2 = dbContext.StudyYears.Where(x => x.IsCurrentYear == true && x.Id != model.Id && x.IsDeleted == false).FirstOrDefault();
                    if (Oldmodel2 != null)
                    {
                        result.Result = Oldmodel2;
                        result.IsSuccess = false;
                        result.Message = "يوجد عام حالي اخر";
                        return result;
                    }
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.IsCurrentYear = model.IsCurrentYear;
                Oldmodel.DisplayOrder = model.DisplayOrder;
                Oldmodel.Notes = model.Notes;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudyYear> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudyYear>();
                var Oldmodel = dbContext.StudyYears.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا العام غير موجود ";
                    return result;
                }
                if (Oldmodel.EmployeeClasses.Any(y => y.IsDeleted == false) || Oldmodel.EmployeesWorkShifts.Any(y => y.IsDeleted == false) || Oldmodel.Expenses.Any(y => y.IsDeleted == false) || Oldmodel.Revenues.Any(y => y.IsDeleted == false) || Oldmodel.StudentsClasses.Any(y => y.IsDeleted == false) || Oldmodel.StudyClasses.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = "هذا العام لا يمكن حذفه ";
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
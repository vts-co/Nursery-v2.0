using NurseryProject.Dtos.StudentExamDegrees;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentExamDegrees
{
    public class StudentExamDegreesServices
    {
        public List<StudentExamDegreesDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.ClassExams.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new StudentExamDegreesDto
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
                    ExamId = x.ExamId.Value,
                    ExamName = x.Exam.Subject.Name + "/" + x.Exam.ExamsType.Name + "/" + x.Exam.Subject.Level.Name,
                    IsOneQuestion=x.Exam.IsOneQuestion.Value,
                    Students = x.StudentsExamDegrees.Where(y => y.IsDeleted == false).OrderBy(y => y.CreatedOn).Select(y => new StudentExamDegreesDetailsDto
                    {
                        Id = y.Id,
                        StudentId = y.StudentId.Value,
                        StudentName = y.Student.Name,
                        TotalDegree = x.StudentsExamDegrees.Where(z => z.IsDeleted == false && z.StudentId == y.StudentId).Sum(z => z.Degree).ToString(),
                        Date = y.Date.Value.ToString(),

                    }).ToList()
                }).ToList();
                foreach (var item in model)
                {
                    var student = item.Students[0].StudentId;
                    if (item.Students.Count() > 1)
                    {
                        for (int i = 1; i < item.Students.Count(); i++)
                        {
                            if (item.Students[i].StudentId == student)
                            {
                                model.Remove(item);
                            }
                            else
                            {
                                student = item.Students[i].StudentId;
                            }
                        }

                    }

                }
                return model;
            }
        }

        public ResultDto<StudentExamDegreesDto> Create(StudentExamDegreesDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentExamDegreesDto>();

                if (model.Students != null)
                {
                    ClassExam classExams = new ClassExam();
                    classExams.Id = Guid.NewGuid();
                    classExams.ExamId = model.ExamId;
                    classExams.ClassId = model.ClassId;

                    classExams.CreatedOn = DateTime.UtcNow;
                    classExams.CreatedBy = UserId;
                    classExams.IsDeleted = false;
                    dbContext.ClassExams.Add(classExams);

                    foreach (var item in model.Students)
                    {
                        StudentsExamDegree studentExamDegree = new StudentsExamDegree();
                        studentExamDegree.Id = Guid.NewGuid();
                        studentExamDegree.ClassExamId = classExams.Id;
                        if (item.ExamDegreeId != Guid.Empty)
                            studentExamDegree.ExamDegreeId = item.ExamDegreeId;
                       
                        studentExamDegree.StudentId = item.StudentId;
                        studentExamDegree.Degree = float.Parse(item.TotalDegree);
                        studentExamDegree.Date = DateTime.Parse(item.Date).Date;
                        studentExamDegree.CreatedOn = DateTime.UtcNow;
                        studentExamDegree.CreatedBy = UserId;
                        studentExamDegree.IsDeleted = false;

                        dbContext.StudentsExamDegrees.Add(studentExamDegree);
                        dbContext.SaveChanges();
                    }
                }
                dbContext.SaveChanges();

                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<StudentExamDegreesDto> Edit(StudentExamDegreesDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentExamDegreesDto>();
                var Oldmodel = dbContext.ClassExams.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الاختبار غير موجود ";
                    return result;
                }
                var Oldmodel2 = dbContext.StudentsExamDegrees.Where(x => x.IsDeleted == false && x.ClassExamId == model.Id).ToList();
                if (Oldmodel2 != null)
                {
                    foreach (var item in Oldmodel2)
                    {
                        item.IsDeleted = false;
                        item.DeletedBy = UserId;
                        item.DeletedOn = DateTime.UtcNow;
                        dbContext.SaveChanges();
                    }
                }

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.ExamId = model.ExamId;
                Oldmodel.ClassId = model.ClassId;
                //Oldmodel.Date = DateTime.Parse(model.Date);

                dbContext.SaveChanges();

                foreach (var item in model.Students)
                {
                    StudentsExamDegree studentExamDegree = new StudentsExamDegree();
                    studentExamDegree.Id = Guid.NewGuid();
                    studentExamDegree.ClassExamId = model.Id;
                    studentExamDegree.ExamDegreeId = item.ExamDegreeId;
                    studentExamDegree.StudentId = item.StudentId;
                    studentExamDegree.Degree = float.Parse(item.TotalDegree);
                    studentExamDegree.Date = DateTime.Parse(item.Date).Date;
                    studentExamDegree.CreatedOn = DateTime.UtcNow;
                    studentExamDegree.CreatedBy = UserId;
                    studentExamDegree.IsDeleted = false;
                    dbContext.StudentsExamDegrees.Add(studentExamDegree);
                    dbContext.SaveChanges();
                }

                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<ClassExam> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ClassExam>();
                var Oldmodel2 = dbContext.StudentsExamDegrees.Where(x => x.IsDeleted == false && x.ClassExamId == Id).ToList();
                if (Oldmodel2 != null)
                {
                    foreach (var item in Oldmodel2)
                    {
                        item.IsDeleted = false;
                        item.DeletedBy = UserId;
                        item.DeletedOn = DateTime.UtcNow;
                        dbContext.SaveChanges();
                    }
                }
                var Oldmodel = dbContext.ClassExams.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الاختبار غير موجود ";
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
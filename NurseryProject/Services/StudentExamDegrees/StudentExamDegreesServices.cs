using NurseryProject.Dtos.Exams;
using NurseryProject.Dtos.StudentExamDegrees;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.StudentExamDegrees
{
    public class StudentExamDegreesServices
    {
        public List<StudentExamDegreesDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.ClassExams.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new StudentExamDegreesDto
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
                    ExamName = x.Exam.IsOneQuestion == true ? x.Exam.Subject.Name + "/" + x.Exam.ExamsType.Name + "/" + "سؤال" + "/" + x.Exam.TotalDegree : x.Exam.Subject.Name + "/" + x.Exam.ExamsType.Name + "/" + "عدة اسئلة" + "/" + x.Exam.TotalDegree,
                    ExamDegree = x.Exam.TotalDegree.ToString(),
                    IsOneQuestion = x.Exam.IsOneQuestion.Value,
                    Students = x.StudentsExamDegrees.Where(y => y.IsDeleted == false).OrderBy(y => y.CreatedOn).Select(y => new StudentExamDegreesDetailsDto
                    {
                        Id = y.Id,
                        Code = y.Student.Code,

                        StudentId = y.StudentId.Value,
                        StudentName = y.Student.Name,
                        TotalDegree = x.StudentsExamDegrees.Where(z => z.IsDeleted == false && z.StudentId == y.StudentId).Sum(z => z.Degree).ToString(),
                        Date = y.Date.Value.ToString(),

                    }).ToList()
                }).ToList();

                foreach (var item in model)
                {
                    if (item.Students.Count() > 1)
                    {
                        var student = item.Students[0].StudentId;
                        var count = item.Students.Count();
                        for (int i = 1; i < count; i++)
                        {
                            if (item.Students[i].StudentId == student)
                            {
                                item.Students.Remove(item.Students[i]);
                                i--;
                                count -= 1;
                            }
                            else
                            {
                                student = item.Students[i].StudentId;
                            }
                        }

                    }
                    item.Count = item.Students.Count().ToString();
                }
                return model;
            }
        }

        public List<ClassStudentsResultDto> GetClassStudentsResultAll(Guid UserId, Guid EmployeeId, Role RoleId, Guid ClassId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {

                List<ClassStudentsResultDto> list = new List<ClassStudentsResultDto>();
                var studentclass = dbContext.StudentsClasses.Where(x => !x.IsDeleted && x.ClassId == ClassId).ToList();
                var Exams = dbContext.ClassExams.Where(x => x.ClassId == ClassId && x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId))).OrderBy(x => x.CreatedOn).ToList();

                foreach (var item in studentclass)
                {
                    ClassStudentsResultDto dto = new ClassStudentsResultDto();
                    dto.Details = new List<ClassStudentsSubjectsResultDto>();
                    dto.StudentName = item.Student.Name;
                    foreach (var item2 in Exams)
                    {
                        ClassStudentsSubjectsResultDto classStudents = new ClassStudentsSubjectsResultDto();
                        var StudentExamDegree = dbContext.StudentsExamDegrees.Where(x => !x.IsDeleted && x.StudentId == item.StudentId && x.ClassExamId == item2.Id).ToList();
                        if (StudentExamDegree.Count() > 0)
                        {
                            classStudents.Degree = StudentExamDegree.Select(x => x.Degree).DefaultIfEmpty(0).Sum();
                            classStudents.ExamDegree = item2.Exam.TotalDegree;
                            dto.Details.Add(classStudents);
                        }
                        else
                        {
                            classStudents.Degree = 0;
                            classStudents.ExamDegree = item2.Exam.TotalDegree;
                            dto.Details.Add(classStudents);
                        }

                    }
                    dto.Total = dto.Details.Select(x => x.Degree).DefaultIfEmpty(0).Sum();
                    dto.Percentage = Math.Round((double)((dto.Total * 100) / dto.Details.Select(x => x.ExamDegree).DefaultIfEmpty(0).Sum()), 2);
                    list.Add(dto);
                }

                return list;
            }
        }
        public List<YearClassResultDto> GetYearClasssResultAll(Guid YearId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {

                List<YearClassResultDto> list = new List<YearClassResultDto>();

                var classes = dbContext.Classes.Where(x => !x.IsDeleted && x.LevelId == YearId).ToList();
                var total = 0.0;
                double totalclassdegree=0.0;
                double ExamDegree=0.0;
                var Count = 0;

                foreach (var item3 in classes)
                {
                    var studentclass = dbContext.StudentsClasses.Where(x => !x.IsDeleted&&x.ClassId==item3.Id).ToList();
                    YearClassResultDto dto1 = new YearClassResultDto();
                    dto1.ClassName = item3.Name;
                    foreach (var item in studentclass)
                    {
                        var Exams = dbContext.ClassExams.Where(x => x.IsDeleted == false && x.ClassId == item3.Id).OrderBy(x => x.CreatedOn).ToList();
                  
                        foreach (var item2 in Exams)
                        {
                            ClassStudentsSubjectsResultDto classStudents = new ClassStudentsSubjectsResultDto();
                            var StudentExamDegree = dbContext.StudentsExamDegrees.Where(x => !x.IsDeleted && x.StudentId == item.StudentId && x.ClassExamId == item2.Id).ToList();
                            if (StudentExamDegree.Count() > 0)
                            {
                                totalclassdegree+=(double)StudentExamDegree.Select(x => x.Degree).DefaultIfEmpty(0).Sum();
                            }
                            ExamDegree += (double)item2.Exam.TotalDegree;


                        }
                 
                        total += Math.Round(((totalclassdegree * 100) / ExamDegree), 2);
                        ExamDegree = 0;
                        totalclassdegree = 0;
                        Count += 1;

                    }
                    dto1.Persntage =Math.Round( total/ Count,2);
                    ExamDegree = 0;
                    total = 0;
                    Count = 0;
                    list.Add(dto1);
                }




                return list;
            }
        }
        public List<Data> GetAllClassExams(Guid UserId, Guid EmployeeId, Role RoleId, Guid ClassId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {

                var Exams = dbContext.ClassExams.Where(x => x.ClassId == ClassId && x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId))).OrderBy(x => x.CreatedOn)

                    .Select(x => new Data { Name = x.Exam.Subject.Name + " | " + x.Exam.TotalDegree, Degree = x.Exam.TotalDegree }).ToList();


                return Exams;
            }
        }


        public List<StudentExamDegreesDto> GetAll2(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsExamDegrees.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.ClassExam.Class.EmployeeClasses.Any(y => y.IsDeleted == false && y.Id == EmployeeId) || x.ClassExam.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.Id == EmployeeId))).OrderBy(x => x.CreatedOn).Select(x => new StudentExamDegreesDto
                {
                    Id = x.Id,
                    StudyPlaceId = x.ClassExam.Class.StudyPlaceId.Value,
                    StudyPlaceName = x.ClassExam.Class.StudyPlace.Name,
                    StudentPhone = x.Student.Phone,

                    StudyTypeId = x.ClassExam.Class.Level.StudyTypeId.Value,
                    StudyTypeName = x.ClassExam.Class.Level.StudyType.Name,
                    LevelId = x.ClassExam.Class.LevelId.Value,
                    LevelName = x.ClassExam.Class.Level.Name,
                    ClassId = x.ClassExam.ClassId.Value,
                    ClassName = x.ClassExam.Class.Name,
                    ExamId = x.ClassExam.ExamId.Value,
                    ExamName = x.ClassExam.Exam.IsOneQuestion == true ? x.ClassExam.Exam.Subject.Name + "/" + x.ClassExam.Exam.ExamsType.Name + "/" + "سؤال" + "/" + x.ClassExam.Exam.TotalDegree : x.ClassExam.Exam.Subject.Name + "/" + x.ClassExam.Exam.ExamsType.Name + "/" + "عدة اسئلة" + "/" + x.ClassExam.Exam.TotalDegree,
                    ExamDegree = x.ClassExam.Exam.TotalDegree.ToString(),
                    IsOneQuestion = x.ClassExam.Exam.IsOneQuestion.Value,
                    Code = x.Student.Code,
                    StudentId = x.StudentId.Value,
                    StudentName = x.Student.Name,
                    TotalDegree = x.Degree.ToString(),

                    Date = x.Date.Value.ToString(),

                }).ToList();

                foreach (var item in model)
                {
                    if (item.Students.Count() > 1)
                    {
                        var student = item.Students[0].StudentId;
                        var count = item.Students.Count();
                        for (int i = 1; i < count; i++)
                        {
                            if (item.Students[i].StudentId == student)
                            {
                                item.Students.Remove(item.Students[i]);
                                i--;
                                count -= 1;
                            }
                            else
                            {
                                student = item.Students[i].StudentId;
                            }
                        }

                    }
                    item.Count = item.Students.Count().ToString();
                }
                return model;
            }
        }

        public List<StudentExamDegreesDetailsDto> StudentExamDegrees(Guid Id, Guid StudentId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.StudentsExamDegrees.Where(y => y.IsDeleted == false && y.ClassExamId == Id && y.StudentId == StudentId).OrderBy(y => y.CreatedOn).Select(y => new StudentExamDegreesDetailsDto
                {
                    Id = y.Id,
                    Code = y.Student.Code,
                    StudentId = y.StudentId.Value,
                    StudentName = y.Student.Name,
                    ExamDegreeId = y.ExamDegreeId != null ? y.ExamDegreeId.Value : Guid.Empty,
                    TotalDegree = y.ExamDegreeId != null ? y.Degree.Value.ToString() : "",
                    Date = y.Date.Value.ToString()

                }).ToList();

                return model;
            }
        }
        public ResultDto<StudentExamDegreesDto> Create(StudentExamDegreesDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<StudentExamDegreesDto>();


                ClassExam classExams = new ClassExam();
                classExams.Id = Guid.NewGuid();
                classExams.ExamId = model.ExamId;
                classExams.ClassId = model.ClassId;

                classExams.CreatedOn = DateTime.UtcNow;
                classExams.CreatedBy = UserId;
                classExams.IsDeleted = false;
                dbContext.ClassExams.Add(classExams);
                if (model.Students != null)
                {
                    foreach (var item in model.Students)
                    {
                        if (item.TotalDegree != null)
                        {
                            StudentsExamDegree studentExamDegree = new StudentsExamDegree();
                            studentExamDegree.Id = Guid.NewGuid();
                            studentExamDegree.ClassExamId = classExams.Id;
                            if (item.ExamDegreeId != Guid.Empty)
                                studentExamDegree.ExamDegreeId = item.ExamDegreeId;

                            studentExamDegree.StudentId = item.StudentId;
                            studentExamDegree.Degree = float.Parse(item.TotalDegree);
                            if (model.Students[0].Date != null)
                                studentExamDegree.Date = DateTime.Parse(model.Students[0].Date).Date;
                            studentExamDegree.CreatedOn = DateTime.UtcNow;
                            studentExamDegree.CreatedBy = UserId;
                            studentExamDegree.IsDeleted = false;

                            dbContext.StudentsExamDegrees.Add(studentExamDegree);
                            dbContext.SaveChanges();
                        }

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
                        item.IsDeleted = true;
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

                if (model.Students != null)
                {
                    foreach (var item in model.Students)
                    {
                        if (item.TotalDegree != null)
                        {
                            StudentsExamDegree studentExamDegree = new StudentsExamDegree();
                            studentExamDegree.Id = Guid.NewGuid();
                            studentExamDegree.ClassExamId = model.Id;
                            if (item.ExamDegreeId != Guid.Empty)
                                studentExamDegree.ExamDegreeId = item.ExamDegreeId;

                            studentExamDegree.StudentId = item.StudentId;
                            studentExamDegree.Degree = float.Parse(item.TotalDegree);
                            studentExamDegree.Date = DateTime.Parse(model.Students[0].Date).Date;
                            studentExamDegree.CreatedOn = DateTime.UtcNow;
                            studentExamDegree.CreatedBy = UserId;
                            studentExamDegree.IsDeleted = false;
                            dbContext.StudentsExamDegrees.Add(studentExamDegree);
                            dbContext.SaveChanges();
                        }
                    }
                }

                dbContext.SaveChanges();

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
                var Oldmodel = dbContext.ClassExams.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "هذا الاختبار غير موجود ";
                    return result;
                }

                var Oldmodel2 = dbContext.StudentsExamDegrees.Where(x => x.IsDeleted == false && x.ClassExamId == Id).ToList();

                if (Oldmodel2 != null)
                {
                    foreach (var item in Oldmodel2)
                    {
                        item.IsDeleted = true;
                        item.DeletedBy = UserId;
                        item.DeletedOn = DateTime.UtcNow;
                        dbContext.SaveChanges();
                    }
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
    public class Data
    {
        public string Name { get; set; }
        public double? Degree { get; set; }
    }
}
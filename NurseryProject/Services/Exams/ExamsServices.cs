using NurseryProject.Dtos.Exams;
using NurseryProject.Enums;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.Exams
{
    public class ExamsServices
    {
        public List<ExamsDto> GetAll(Guid UserId, Guid EmployeeId, Role RoleId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Exams.Where(x => x.IsDeleted == false && (x.CreatedBy == UserId || RoleId == Role.SystemAdmin || x.ClassExams.Any(y=>y.IsDeleted==false&&y.Class.EmployeeClasses.Any(z=>z.IsDeleted==false&&z.EmployeeId== EmployeeId)) || x.ClassExams.Any(y => y.IsDeleted == false && y.Class.ClassesLeaders.Any(z => z.IsDeleted == false && z.EmployeeId == EmployeeId)))).OrderBy(x => x.CreatedOn).Select(x => new ExamsDto
                {
                    Id = x.Id,
                    Name = x.IsOneQuestion == true ? x.Subject.Name + "/" + x.ExamsType.Name + "/" + "سؤال" + "/" + x.TotalDegree : x.Subject.Name + "/" + x.ExamsType.Name + "/" + "عدة اسئلة" + "/" + x.TotalDegree,
                    StudyTypeId = x.Subject.Level.StudyType.Id,
                    StudyTypeName = x.Subject.Level.StudyType.Name,
                    LevelId = x.Subject.Level.Id,
                    LevelName = x.Subject.Level.Name,
                    SubjectId = x.Subject.Id,
                    SubjectName = x.Subject.Name,
                    ExamTypeId = x.ExamsType.Id,
                    ExamTypeName = x.ExamsType.Name,
                    IsOneQuestion = x.IsOneQuestion.Value,
                    TotalDegree = x.TotalDegree.ToString(),

                    MoreQuestion = x.ExamDegrees.Where(y => y.IsDeleted == false && y.ExamId == x.Id).OrderBy(y => y.CreatedOn).Select(y => new MoreQuestionDto
                    {
                        Id = y.Id,
                        ExamId = x.Id,
                        ExamName = x.Subject.Name + " / " + x.ExamsType.Name,
                        Degree = y.Degree,

                    }).ToList(),


                }).ToList();

                return model;
            }
        }
        public ExamsDto Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Exams.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).Select(x => new ExamsDto
                {
                    Id = x.Id,
                    StudyTypeId = x.Subject.Level.StudyTypeId.Value,
                    StudyTypeName = x.Subject.Level.StudyType.Name,
                    LevelId = x.Subject.LevelId.Value,
                    LevelName = x.Subject.Level.Name,
                    SubjectId = x.Subject.Id,
                    SubjectName = x.Subject.Name,
                    ExamTypeId = x.ExamsType.Id,
                    ExamTypeName = x.ExamsType.Name,
                    IsOneQuestion = x.IsOneQuestion.Value,
                    TotalDegree = x.TotalDegree.ToString(),

                    MoreQuestion = x.ExamDegrees.Where(y => y.IsDeleted == false && y.ExamId == x.Id).OrderBy(y => y.CreatedOn).Select(y => new MoreQuestionDto
                    {
                        Id = y.Id,
                        ExamId = x.Id,
                        ExamName = x.Subject.Name + " / " + x.ExamsType.Name,
                        Degree = y.Degree,

                    }).ToList(),


                }).FirstOrDefault();

                return model;
            }
        }
        public ResultDto<ExamsDto> Create(ExamsDto model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ExamsDto>();
                //var Oldmodel = dbContext.Exams.Where(x => x.ExamTypeId == model.ExamTypeId && x.IsDeleted == false && x.SubjectId == model.SubjectId).FirstOrDefault();
                //if (Oldmodel != null)
                //{
                //    result.IsSuccess = false;
                //    result.Message = "هذا الاختبار موجود بالفعل";
                //    return result;
                //}
                
                var Exam = new Models.Exam()
                {
                    Id = model.Id,
                    SubjectId = model.SubjectId,
                    ExamTypeId = model.ExamTypeId,
                    TotalDegree = float.Parse(model.TotalDegree),
                    IsOneQuestion = model.IsOneQuestion,

                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = UserId,
                    IsDeleted = false,
                };
                dbContext.Exams.Add(Exam);
                dbContext.SaveChanges();

                if (model.MoreQuestion != null)
                {
                    var i = 1;
                    foreach (var item in model.MoreQuestion)
                    {
                        var moreQuestion = new ExamDegree()
                        {
                            Id = Guid.NewGuid(),
                            ExamId = model.Id,
                            Degree = item.Degree,
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = UserId,
                            IsDeleted = false,
                        };
                        dbContext.ExamDegrees.Add(moreQuestion);
                        dbContext.SaveChanges();
                        i++;
                    }
                }

                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<ExamsDto> Edit(ExamsDto model,Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ExamsDto>();
                var Oldmodel = dbContext.Exams.Find(model.Id);
                

                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.SubjectId = model.SubjectId;
                Oldmodel.ExamTypeId = model.ExamTypeId;
                Oldmodel.TotalDegree = float.Parse(model.TotalDegree);
                Oldmodel.IsOneQuestion = model.IsOneQuestion;

                var examDegrees = dbContext.ExamDegrees.Where(x => x.ExamId == model.Id && x.IsDeleted == false).ToList();
                if (examDegrees.Count() > 0)
                {

                    foreach (var item in examDegrees)
                    {
                        item.IsDeleted = true;
                        item.DeletedBy = UserId;
                        item.DeletedOn = DateTime.UtcNow;
                        dbContext.SaveChanges();
                    }
                }
                if (model.MoreQuestion != null)
                {
                    var i = 1;

                    foreach (var item in model.MoreQuestion)
                    {
                        var examDegrees1 = new ExamDegree();
                        {
                            examDegrees1.Id = Guid.NewGuid();
                            examDegrees1.ExamId = model.Id;
                            examDegrees1.Degree = item.Degree;

                            examDegrees1.CreatedOn = DateTime.UtcNow;
                            examDegrees1.CreatedBy = UserId;
                            examDegrees1.IsDeleted = false;
                        };
                        dbContext.ExamDegrees.Add(examDegrees1);
                        dbContext.SaveChanges();
                        i++;
                    }
                }
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<ExamsDto> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<ExamsDto>();
                var Oldmodel = dbContext.Exams.Find(Id);
                var examDegrees = dbContext.ExamDegrees.Where(x => x.ExamId == Id && x.IsDeleted == false).ToList();
                if (examDegrees.Count() > 0)
                {
                    foreach (var item in examDegrees)
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
}
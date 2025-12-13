using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NurseryProject.Dtos.Supplier;
namespace NurseryProject.Services.Suppliers
{
    public class SupplierServices
    {
        public List<SupplierDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Suppliers.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x=>new SupplierDto
                { 
                    Id=x.Id,
                    Name=x.Name,
                    GroupName=x.SupplierGroup.Name,
                    Phone=x.Phone
                
                }).ToList();
                return model;
            }
        }
        public Supplier Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.Suppliers.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<Supplier> Create(Supplier model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Supplier>();
                var Oldmodel = dbContext.Suppliers.Where(x => x.Name == model.Name && x.IsDeleted == false).FirstOrDefault();
                if (Oldmodel != null)
                {
                    result.Result = Oldmodel;
                    result.IsSuccess = false;
                    result.Message = "المورد موجود بالفعل";
                    return result;
                }
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.Suppliers.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Supplier> Edit(Supplier model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Supplier>();
                var Oldmodel = dbContext.Suppliers.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "المورد غير موجود ";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Name = model.Name;
                Oldmodel.Phone = model.Phone;
                Oldmodel.GroupId = model.GroupId;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<Supplier> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<Supplier>();
                var Oldmodel = dbContext.Suppliers.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "المورد غير موجود ";
                    return result;
                }

                if (Oldmodel.PurchaseInvoices.Any(y => y.IsDeleted == false))
                {
                    result.IsSuccess = false;
                    result.Message = " لا يمكن حذف المورد لارتباطه بعمليات";
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
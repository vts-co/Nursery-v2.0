using NurseryProject.Dtos.Supplier;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurseryProject.Services.SupplierPayments
{
    public class SupplierPayementServices
    {

        public List<SupplierPaymentDto> GetAll()
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SupplierPayments.Where(x => x.IsDeleted == false).OrderBy(x => x.CreatedOn).Select(x => new SupplierPaymentDto
                {
                    Id = x.Id,
                    Name = x.Supplier.Name,
                    Amount = x.Amount,
                    PaymentDate = x.PaymentDate

                }).ToList();
                return model;
            }
        }
        public SupplierPayment Get(Guid Id)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var model = dbContext.SupplierPayments.Where(x => x.IsDeleted == false && x.Id == Id).OrderBy(x => x.CreatedOn).FirstOrDefault();
                return model;
            }
        }
        public ResultDto<SupplierPayment> Create(SupplierPayment model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<SupplierPayment>();
            
                model.CreatedOn = DateTime.UtcNow;
                model.CreatedBy = UserId;
                model.IsDeleted = false;
                dbContext.SupplierPayments.Add(model);
                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم حفظ البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<SupplierPayment> Edit(SupplierPayment model, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<SupplierPayment>();
                var Oldmodel = dbContext.SupplierPayments.Find(model.Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "دفعة المورد غير موجوده ";
                    return result;
                }
                Oldmodel.ModifiedOn = DateTime.UtcNow;
                Oldmodel.ModifiedBy = UserId;
                Oldmodel.Amount = model.Amount;
                Oldmodel.PaymentDate = model.PaymentDate;
                Oldmodel.SupplierId = model.SupplierId;

                dbContext.SaveChanges();
                result.IsSuccess = true;
                result.Message = "تم تعديل البيانات بنجاح";
                return result;
            }
        }
        public ResultDto<SupplierPayment> Delete(Guid Id, Guid UserId)
        {
            using (var dbContext = new almohandes_DbEntities())
            {
                var result = new ResultDto<SupplierPayment>();
                var Oldmodel = dbContext.SupplierPayments.Find(Id);
                if (Oldmodel == null)
                {
                    result.IsSuccess = false;
                    result.Message = "دفعة المورد غير موجوده ";
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
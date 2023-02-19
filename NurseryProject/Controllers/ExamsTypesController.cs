using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.ExamsTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "24")]

    public class ExamsTypesController : Controller
    {
        ExamsTypesServices examsTypesServices = new ExamsTypesServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = examsTypesServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            
            return View("Upsert", new ExamsType());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(ExamsType examsType)
        {
            examsType.Id = Guid.NewGuid();
            var result = examsTypesServices.Create(examsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                examsType.Id = Guid.Empty;
               
                TempData["warning"] = result.Message;
                return View("Upsert", examsType);
            }
        }
        public ActionResult Edit(Guid Id)
        {
          
            var examsType = examsTypesServices.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return View("Upsert", examsType);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(ExamsType examsType)
        {

            var result = examsTypesServices.Edit(examsType, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                
                TempData["warning"] = result.Message;
                return View("Upsert", examsType);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = examsTypesServices.Delete(Id, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
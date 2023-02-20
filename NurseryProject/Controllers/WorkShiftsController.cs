using NurseryProject.Authorization;
using NurseryProject.Dtos.WorkShifts;
using NurseryProject.Enums;
using NurseryProject.Models;
using NurseryProject.Services.WorkShifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "18")]

    public class WorkShiftsController : Controller
    {
        WorkShiftsServices workShiftsServices = new WorkShiftsServices();
        // GET: Cities
        public ActionResult Index()
        {
            var model = workShiftsServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            return View("Upsert", new WorkShiftsDto());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(WorkShiftsDto workShiftsDto)
        {
            var result = workShiftsServices.Create(workShiftsDto, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                workShiftsDto.Id = Guid.Empty;

                TempData["warning"] = result.Message;
                return View("Upsert", workShiftsDto);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var workShiftsDto = workShiftsServices.Get(Id);

            return View("Upsert", workShiftsDto);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(WorkShiftsDto workShiftsDto)
        {
            var result = workShiftsServices.Edit(workShiftsDto, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["warning"] = result.Message;
                return View("Upsert", workShiftsDto);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = workShiftsServices.Delete(Id, (Guid)TempData["UserId"]);
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
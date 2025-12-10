using NurseryProject.Authorization;
using NurseryProject.Models;
using NurseryProject.Services.ItemGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "4")]

    public class ItemGroupsController : Controller
    {
        ItemGroupServices GroupServices = new ItemGroupServices();
        // GET: ItemGroups
        public ActionResult Index()
        {
            var model = GroupServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
          

            return View("Upsert", new ItemGroup());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(ItemGroup ItemGroup)
        {
            ItemGroup.Id = Guid.NewGuid();
            var result = GroupServices.Create(ItemGroup, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                ItemGroup.Id = Guid.Empty;
            

                TempData["warning"] = result.Message;
                return View("Upsert", ItemGroup);
            }
        }
        public ActionResult Edit(Guid Id)
        {
          

            var ItemGroup = GroupServices.Get(Id);
            return View("Upsert", ItemGroup);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(ItemGroup ItemGroup)
        {

            var result = GroupServices.Edit(ItemGroup, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
               

                TempData["warning"] = result.Message;
                return View("Upsert", ItemGroup);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = GroupServices.Delete(Id, (Guid)TempData["UserId"]);
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
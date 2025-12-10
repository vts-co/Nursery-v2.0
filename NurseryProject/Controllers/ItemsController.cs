using NurseryProject.Authorization;
using NurseryProject.Services.ItemGroups;
using NurseryProject.Services.Items;
using NurseryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "4")]

    public class ItemsController : Controller
    {
        ItemServices itemServices = new ItemServices();
        ItemGroupServices ItemGroup = new ItemGroupServices();
        // GET: Items
        // GET: Cities
        public ActionResult Index()
        {
            var model = itemServices.GetAll();
            return View(model);
        }
        public ActionResult Create()
        {
            var Groups = ItemGroup.GetAll();
            ViewBag.Groups = Groups;

            return View("Upsert", new Item());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Item Item)
        {
            Item.Id = Guid.NewGuid();
            var result = itemServices.Create(Item, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                Item.Id = Guid.Empty;
                var Groups = ItemGroup.GetAll();
                ViewBag.Groups = Groups;

                TempData["warning"] = result.Message;
                return View("Upsert", Item);
            }
        }
        public ActionResult Edit(Guid Id)
        {
            var Groups = ItemGroup.GetAll();
            ViewBag.Groups = Groups;

            var Item = itemServices.Get(Id);
            return View("Upsert", Item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Item Item)
        {

            var result = itemServices.Edit(Item, (Guid)TempData["UserId"]);
            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction("Index");
            }
            else
            {
                var Groups = ItemGroup.GetAll();
                ViewBag.Groups = Groups;

                TempData["warning"] = result.Message;
                return View("Upsert", Item);
            }
        }
        public ActionResult Delete(Guid Id)
        {
            var result = itemServices.Delete(Id, (Guid)TempData["UserId"]);
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
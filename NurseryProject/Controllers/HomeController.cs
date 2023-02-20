using NurseryProject.Authorization;
using NurseryProject.Dtos.HomePage;
using NurseryProject.Enums;
using NurseryProject.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "0")]

    public class HomeController : Controller
    {

        SettingsServices settingsServices = new SettingsServices();
        public ActionResult Index()
        {
            List<HomePagesDro> homePages = new List<HomePagesDro>();
            var homeScreens = settingsServices.GetAllHomePages();
            var userPages = ViewBag.UserScreens;

            foreach (var item in homeScreens)
            {
                var screenId = "," + item.PageId + ",";

                if (userPages.Contains(screenId))
                {
                    var page = settingsServices.GetPage(item.PageId.Value);

                    homePages.Add(new HomePagesDro
                    {
                        Id = page.Id,
                        Name = page.Name,
                        Icon = page.Icone,
                        Link = page.Link
                    });
                    
                }
            }
            return View(homePages);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
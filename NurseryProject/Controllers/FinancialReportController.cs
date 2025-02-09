using NurseryProject.Authorization;
using NurseryProject.Enums;
using NurseryProject.Services.Revenues;
using NurseryProject.Services.StudyPlaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NurseryProject.Controllers
{
    [Authorized(ScreenId = "30")]

    public class FinancialReportController : Controller
    {
        StudyPlacesServices studyPlacesServices = new StudyPlacesServices();
        FinancialReportServices financialReportServices = new FinancialReportServices();
        // GET: FinancialReport
        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Index(string date)
        {
           

            var model = financialReportServices.GetFinancial(date);
            return View(model);
        }
    }
}
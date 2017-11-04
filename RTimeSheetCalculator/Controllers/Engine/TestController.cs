using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RTimeSheetCalculator.Controllers.Engine
{
    public class TestController : Controller
    {
        public ActionResult DailyTimeSheet() {
            return View();
        }
    }
}
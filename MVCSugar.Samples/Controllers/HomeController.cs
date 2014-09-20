using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCSugar.Samples.Models;

namespace MVCSugar.Samples.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new EnumSample() { Hero = Heroes.WonderWoman });
        }
    }
}
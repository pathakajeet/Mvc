using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientEnquiry.Controllers
{
    public class InfoController : Controller
    {       
        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}
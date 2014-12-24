using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DYH.Web.Controllers
{
    public class UsersController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }
    }
}
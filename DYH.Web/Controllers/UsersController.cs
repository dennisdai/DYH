using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DYH.IDAL;

namespace DYH.Web.Controllers
{
    public class UsersController : Controller
    {
        private IUser _user;
        public UsersController(IUser user)
        {
            _user = user;
        }

        public ActionResult Login()
        {
            var list = _user.GetList();
            var lst = list.ToList();

            return View();
        }
    }
}
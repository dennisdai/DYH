using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DYH.Core;
using DYH.IDAL;
using DYH.Models;
using DYH.Models.ViewModel;
using DYH.Web.Framework.Utils;

namespace DYH.Web.Controllers
{
    public class UsersController : Controller
    {
        private IUser _user;
        public UsersController(IUser user)
        {
            _user = user;
        }

        [HttpGet]
        public ActionResult Login(string type, string ReturnUrl)
        {
            Utility.GetModelState(this);

            var model = new LoginViewModel();

            string langKey = "en-US";
            var cookie = Request.Cookies["DYH.COOKIES"];
            if (cookie == null || string.IsNullOrEmpty(cookie["Language"]))
            {
                //从浏览器获取首选语言
                var firstLangOfBrowser = Request.UserLanguages != null ? Request.UserLanguages[0] : "en-US";
                if (!string.IsNullOrEmpty(firstLangOfBrowser))
                {
                    langKey = firstLangOfBrowser;
                }
            }
            else
            {
                langKey = cookie["Language"];
                model.ReturnUrl = ReturnUrl;
                model.Remember = "on";
                model.UserName = cookie["UserName"];
            }
            
            if (Request.Cookies["LanguageKey"] == null)
            {
                var langCookie = new HttpCookie("LanguageKey") { Value = langKey };
                Response.Cookies.Add(langCookie);
            }

            ViewBag.ReturnUrl = ReturnUrl;
            if (Utility.IsLogin)
            {
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
            }

            if (type == "timeout")
            {
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("", "Please enter user name");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Please enter password");
            }

            if (ModelState.IsValid)
            {
                var info = _user.GetUser(model.UserName);
                string pwd = Security.PwdMd5(model.Password);
                if (info.Password == pwd)
                {
                    string langKey = "en-US";
                    var cookie = Request.Cookies["DYH.COOKIES"];
                    if (cookie != null && !string.IsNullOrEmpty(cookie["Language"]))
                    {
                        langKey = cookie["Language"];
                    }
                    else
                    {
                        //从浏览器获取首选语言
                        var firstLangOfBrowser = Request.UserLanguages != null ? Request.UserLanguages[0] : "en-US";
                        if (!string.IsNullOrEmpty(firstLangOfBrowser))
                        {
                            langKey = firstLangOfBrowser;
                        }
                    }

                    var login = new LoginModel()
                    {
                        UserId = info.UserId,
                        UserName = info.UserName,
                        SessionID = Session.SessionID,
                        Remember = model.Remember,
                        Language = langKey,
                        ClientTimeZone = model.TimezoneOffset
                    };


                    var isRemember = model.Remember == "on";
                    if (isRemember)
                    {
                        if (cookie == null)
                        {
                            cookie = new HttpCookie("DYH.COOKIES") { HttpOnly = true };
                            cookie.Values.Add("UserName", model.UserName);
                            cookie.Values.Add("Language", langKey);
                            cookie.Expires = DateTime.Now.AddDays(7.0);
                            Response.Cookies.Add(cookie);
                        }
                    }
                    else
                    {
                        if (cookie != null)
                        {
                            cookie.Expires = DateTime.Now;
                        }
                    }

                    var serialize = Newtonsoft.Json.JsonConvert.SerializeObject(login);
                    FormsAuthentication.SetAuthCookie(serialize, false);


                    var returnUrl = model.ReturnUrl;
                    return Redirect(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/Dashboard/Index");
                }

                ModelState.AddModelError("", "Bad user name or password");
                Utility.SetErrorModelState(this);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return Redirect("~/Users/Login");
        }

        [Authorize]
        public ActionResult Logout()
        {
            if (Utility.IsLogin)
            {
                FormsAuthentication.SignOut();
            }

            return RedirectToAction("Login");
        }


        public ActionResult Index(int id = 1)
        {
            int records = 0;
            var list = _user.GetList("WHERE 1=1", Utility.PageSize, id, out records);
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(UserEntry model)
        {
            return View();
        }
    }
}
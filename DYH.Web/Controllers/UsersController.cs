using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DYH.Core;
using DYH.IDAL;
using DYH.Models;
using DYH.Models.ViewModel;
using DYH.Web.Framework;
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
                var info = _user.GetUserByName(model.UserName);
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
                        else if (cookie["UserName"] != null)
                        {
                            if (model.UserName != (cookie["UserName"] as string))
                            {
                                Response.Cookies.Remove("DYH.COOKIES");
                                cookie["UserName"] = model.UserName;
                                cookie.Expires = DateTime.Now.AddDays(7.0);
                                Response.Cookies.Add(cookie);
                            }
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


        public ActionResult Index(int page = 1, string SearchBy = "", string SearchContent = "")
        {
            Utility.GetModelState(this);

            ViewBag.SearchBy = SearchBy;
            ViewBag.SearchContext = SearchContent;

            var sbFilter = new StringBuilder();
            if (SearchBy == "UserName")
            {
                sbFilter.AppendFormat("AND username LIKE '%{0}%'", SearchContent);
            }
            else if (SearchBy == "FirstName")
            {
                sbFilter.AppendFormat("AND firstname LIKE '%{0}%'", SearchContent);
            }
            else if (SearchBy == "LastName")
            {
                sbFilter.AppendFormat("AND lastname LIKE '%{0}%'", SearchContent);
            }

            var model = new PageModel
                {
                    Filter = "Where 1=1 " + sbFilter.ToString(),
                    PageIndex = page,
                    PageSize = Utility.PageSize
                };

            var list = _user.GetList(model);
            Pagination.NewPager(this, page, (int)model.Records);
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(UserEntry model)
        {
            if (_user.GetUserByName(model.UserName) != null)
            {
                ModelState.AddModelError("", string.Format("{0} has been used, please change one.", model.UserName));
            }

            if (_user.GetUserByEmail(model.Email) != null)
            {
                ModelState.AddModelError("", string.Format("{0} has been used, please change one.", model.Email));
            }

            if (ModelState.IsValid)
            {
                var info = Utility.CurrentLoginModel;
                model.Password = Security.PwdMd5(model.Password);
                model.CreatedBy = info.UserName;
                model.CreatedTime = DateTime.Now;
                if (Request.Cookies["LanguageKey"] != null)
                    model.Language = Request.Cookies["LanguageKey"].ToString();

                Utility.Operate(this, Operations.Add, () => _user.Add(model), model.UserName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var info = _user.GetUserById(id);
            return View(info);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection collection)
        {
            var email = collection["Email"];
            var info = _user.GetUserById(DataCast.Get<int>(collection["UserId"]));
            var chkEmail = _user.GetUserByEmail(email);

            if (!Validator.IsEmail(email))
            {
                ModelState.AddModelError(string.Empty, "Please enter a valid email.");
            }

            if (chkEmail != null && info.Email != email && !string.IsNullOrEmpty(chkEmail.Email))
            {
                ModelState.AddModelError(string.Empty, string.Format("{0} has been used, please change one.", email));
            }
            else
            {
                if (!string.IsNullOrEmpty(info.Email))
                {
                    info.Email = email;
                }
            }

            if (ModelState.IsValid)
            {
                var password = collection["Password"];
                if (!string.IsNullOrEmpty(password))
                {
                    var md5Pwd = Security.PwdMd5(password);
                    if (info.Password != null && info.Password != md5Pwd)
                    {
                        info.Password = md5Pwd;
                    }
                }

                var first = collection["FirstName"];
                var last = collection["LastName"];

                if (!string.IsNullOrEmpty(first))
                {
                    info.FirstName = first;
                }

                if (!string.IsNullOrEmpty(last))
                {
                    info.LastName = last;
                }

                info.ChangedBy = Utility.CurrentUserName;
                info.ChangedTime = DateTime.UtcNow;

                Utility.Operate(this, Operations.Update, () => _user.Update(info), info.UserName);

                //var roles = RoleService.GetList();
                //var list = new List<UserRoleEntity>();
                //foreach (var item in roles)
                //{
                //    var model = new UserRoleEntity();
                //    var ChkName = "Role_" + item.RoleID;

                //    var ChkVal = collection[ChkName];
                //    var UserRole = "UserRole_" + item.RoleID;
                //    var UserRoleID = DataCast.ToInt(collection[UserRole]);

                //    if (ChkVal == "on")
                //    {
                //        model.RoleID = item.RoleID;
                //        model.UserID = info.UserID;
                //        model.UserRoleID = UserRoleID;
                //    }
                //    else
                //    {
                //        model.RoleID = 0;
                //        model.UserID = 0;
                //        model.UserRoleID = UserRoleID;
                //    }

                //    list.Add(model);
                //}

                //var dt = DataCast.ListToDataTable(list);

                //Utils.Operate(this, OperateTypes.Update, () =>
                //{
                //    return UserService.Update(info, dt);
                //}, info.UserName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            var iVal = 0;
            Utility.Operate(this, Operations.Delete, () =>
            {
                iVal = _user.Delete(id);
                return iVal;
            });

            var page = Pagination.CheckPageIndexWhenDeleted(this, iVal);
            return Redirect(string.Format("~/Users/Index?page={0}", page));
        }

        [Authorize]
        public JsonResult CheckUserName(string userName)
        {
            var flag = true;
            var info = _user.GetUserByName(userName);
            if (info != null)
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult CheckEmail(string email)
        {
            var flag = true;
            var info = _user.GetUserByEmail(email);
            if (info != null)
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}
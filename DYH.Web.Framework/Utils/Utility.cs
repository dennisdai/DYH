using System.Web;
using System.Web.Security;
using DYH.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DYH.Models.ViewModel;

namespace DYH.Web.Framework.Utils
{
    public class Utility
    {
        /// <summary>
        /// get model state if there is some validation errors.
        /// </summary>
        /// <param name="controller">Current controller</param>
        public static void GetModelState(Controller controller)
        {
            if (!DataCast.IsNull(controller.TempData["ModelState"]))
            {
                controller.ViewBag.HasMessage = true;
                var dics = controller.TempData["ModelState"] as ModelStateDictionary;
                if (dics != null)
                    foreach (var item in dics)
                    {
                        if (item.Value != null)
                        {
                            foreach (var error in item.Value.Errors)
                            {
                                controller.ModelState.AddModelError(item.Key, error.ErrorMessage);
                            }
                        }
                    }
            }
        }

        public static void SetErrorModelState(Controller controller)
        {
            controller.TempData["MessageType"] = "error";
            controller.TempData["ModelState"] = controller.ModelState;
        }

        public static bool IsLogin
        {
            get
            {
                if (HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity.IsAuthenticated &&
                    HttpContext.Current.User.Identity is FormsIdentity)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        public static LoginModel CurrentLoginModel
        {
            get
            {
                if (!IsLogin)
                {
                    FormsAuthentication.SignOut();
                    HttpContext.Current.Response.Redirect("~/Users/Login?type=timeout", true);
                    return new LoginModel();
                }

                var model = LoginModel(HttpContext.Current.User.Identity.Name);

                var langCookie = HttpContext.Current.Request.Cookies["LanguageKey"];
                if (DataCast.IsNull(langCookie))
                {
                    langCookie = new HttpCookie("LanguageKey") {Value = model.Language};
                    HttpContext.Current.Response.Cookies.Add(langCookie);
                }

                return model;
            }
        }

        public static LoginModel LoginModel(string cookies)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<LoginModel>(cookies);
            }
            catch
            {
                FormsAuthentication.SignOut();
                return null;
            }
        }
    }
}

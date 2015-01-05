using System.Web;
using System.Web.Security;
using DYH.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DYH.Models;
using DYH.Models.ViewModel;

namespace DYH.Web.Framework.Utils
{
    public class Utility
    {
        public static int PageSize
        {
            get { return 3; }
        }

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

        public static string CurrentUserName
        {
            get { return CurrentLoginModel.UserName; }
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

        /// <summary>
        /// 调用数据库更改数据
        /// </summary>
        /// <param name="controller">当前控制器</param>
        /// <param name="type">操作类型</param>
        /// <param name="dbOperate">执行数据库操作的方法</param>
        /// <param name="message">
        /// <para>根据操作类型传入的消息:</para>
        /// <para>如果操作类型是Add,Save,Delete: 可以传入一个识别更新的表示符号，表示那条记录被操作.</para>
        /// <para>如果操作类型是Custom，提示消息完全自定义.</para>
        /// </param>
        /// <param name="handleError">如果有异常的处理方法</param>
        /// <returns></returns>
        public static ResultEntry Operate(Controller controller, Operations type,
            Func<int> dbOperate,
            string message = "",
            Func<Exception, string> handleError = null)
        {
            var model = new ResultEntry();
            try
            {
                var iVal = dbOperate();

                if (iVal > 0)
                {
                    var lang = string.Empty;
                    switch (type)
                    {
                        case Operations.Add:
                            if (string.IsNullOrEmpty(message))
                            {
                                lang = "Record(s) have been created.";//Localization.GetLang("Record(s) have been created.");
                            }
                            else
                            {
                                lang = string.Format("{0} has been created.", message);
                            }
                            break;
                        case Operations.Update:
                            if (string.IsNullOrEmpty(message))
                            {
                                lang = "Record(s) have been updated.";// Localization.GetLang("Record(s) have been updated.");
                            }
                            else
                            {
                                lang = string.Format("{0} has been updated.", message); // string.Format(Localization.GetLang("{0} has been updated."), recordIdOrCustomMsg);
                            }
                            break;
                        case Operations.Delete:
                            lang = "You selected record(s) has been deleted.";
                            break;
                        case Operations.Save:
                            lang = "Record(s) have been saved.";
                            break;
                        case Operations.Custom:
                            lang = message;
                            break;
                    }

                    controller.ModelState.AddModelError(string.Empty, lang);
                    model.Message = lang;
                }

                model.ResultCode = ResultCodes.OK;
            }
            catch (Exception ex)
            {
                string strMsg;
                if (handleError == null)
                {
                    strMsg = ex.Message;
                    controller.ModelState.AddModelError("Exception", strMsg);
                    controller.TempData["MessageType"] = "error";
                }
                else
                {
                    strMsg = handleError(ex);
                    controller.ModelState.AddModelError("Exception", strMsg);
                    controller.TempData["MessageType"] = "error";
                }
                model.ResultCode = ResultCodes.Exception;
                model.Message = strMsg;
            }

            controller.TempData["ModelState"] = controller.ModelState;

            return model;
        }
    }
}

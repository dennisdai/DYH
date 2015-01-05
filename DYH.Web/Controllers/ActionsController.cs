using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DYH.Core.Caching;
using DYH.IDAL;
using DYH.Models;
using DYH.Web.Framework;
using DYH.Web.Framework.Utils;

namespace DYH.Web.Controllers
{
    public class ActionsController : BaseController
    {
        private readonly IAction _action;
        private readonly ICacheManager _cache;
        public ActionsController(IAction action, ICacheManager cache)
        {
            _action = action;
            _cache = cache;
        }

        public ActionResult Index()
        {
            Utility.GetModelState(this);
            var list = _cache.Get(Constants.CACHE_KEY_ACTIONS, () => _action.GetList());

            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(ActionEntry model)
        {
            var info = _action.GetByCode(model.ActionCode);

            if (info != null && info.ActionCode == model.ActionCode)
            {
                ModelState.AddModelError("ActionCode", string.Format("{0} has been used, please change one.", "Action Code"));
            }

            if (ModelState.IsValid)
            {
                model.CreatedBy = Utility.CurrentUserName;
                model.CreatedTime = DateTime.UtcNow;
                Utility.Operate(this, Operations.Add, () =>
                {
                    _cache.Remove(Constants.CACHE_KEY_ACTIONS);
                    return _action.Add(model);
                }, model.DisplayName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return RedirectToAction("Index", "Actions");
        }

        public ActionResult Edit(int id)
        {
            var list = _cache.Get(Constants.CACHE_KEY_ACTIONS, () => _action.GetList());
            return View(list.FirstOrDefault(x => x.ActionId == id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(ActionEntry model)
        {
            var info = _action.GetById(model.ActionId);
            if (ModelState.IsValid)
            {
                model.CreatedTime = info.CreatedTime;
                model.ChangedBy = info.CreatedBy;
                model.ChangedBy = Utility.CurrentUserName;
                model.ChangedTime = DateTime.UtcNow;
                Utility.Operate(this, Operations.Update, () =>
                {
                    _cache.Remove(Constants.CACHE_KEY_ACTIONS);
                    return _action.Update(model);
                }, model.DisplayName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return RedirectToAction("Index", "Actions");
        }

        public ActionResult Delete(string id)
        {
            var iVal = 0;
            Utility.Operate(this, Operations.Delete, () =>
            {
                iVal = _action.Delete(id);
                return iVal;
            });
            if (iVal > 0)
            {
                _cache.Remove(Constants.CACHE_KEY_ACTIONS);
            }

            return RedirectToAction("Index", "Actions");
        }

        public JsonResult CheckCode(string actionCode)
        {
            var flag = true;
            var info = _action.GetByCode(actionCode);
            if (info != null)
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}

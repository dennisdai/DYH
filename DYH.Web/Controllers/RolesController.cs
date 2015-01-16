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
    public class RolesController : BaseController
    {
        private readonly IRole _role;
        private readonly ICacheManager _cache;
        public RolesController(IRole role, ICacheManager cache)
        {
            _role = role;
            _cache = cache;
        }

        public ActionResult Index()
        {
            Utility.GetModelState(this);
            var list = _cache.Get(Constants.CACHE_KEY_ROLES, () => _role.GetList());

            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(RoleEntry model)
        {
            var info = _role.GetByCode(model.RoleCode);

            if (info != null && info.RoleCode == model.RoleCode)
            {
                ModelState.AddModelError("RoleCode", string.Format("{0} has been used, please change one.", "Action Code"));
            }

            if (ModelState.IsValid)
            {
                model.CreatedBy = Utility.CurrentUserName;
                model.CreatedTime = DateTime.UtcNow;
                Utility.Operate(this, Operations.Add, () =>
                {
                    _cache.Remove(Constants.CACHE_KEY_ROLES);
                    return _role.Add(model);
                }, model.DisplayName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return RedirectToAction("Index", "Roles");
        }

        public ActionResult Edit(int id)
        {
            var list = _cache.Get(Constants.CACHE_KEY_ROLES, () => _role.GetList());
            var model = list.FirstOrDefault(x => x.RoleId == id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(RoleEntry model)
        {
            var info = _role.GetById(model.RoleId);
            if (ModelState.IsValid)
            {
                model.CreatedTime = info.CreatedTime;
                model.ChangedBy = info.CreatedBy;
                model.ChangedBy = Utility.CurrentUserName;
                model.ChangedTime = DateTime.UtcNow;
                Utility.Operate(this, Operations.Update, () =>
                {
                    _cache.Remove(Constants.CACHE_KEY_ROLES);
                    return _role.Update(model);
                }, model.DisplayName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return RedirectToAction("Index", "Roles");
        }

        public ActionResult Delete(string id)
        {
            var iVal = 0;
            Utility.Operate(this, Operations.Delete, () =>
            {
                iVal = _role.Delete(id);
                return iVal;
            });
            if (iVal > 0)
            {
                _cache.Remove(Constants.CACHE_KEY_ROLES);
            }

            return RedirectToAction("Index", "Roles");
        }

        public JsonResult CheckCode(string roleCode)
        {
            var flag = true;
            var info = _role.GetByCode(roleCode);
            if (info != null)
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DYH.Core;
using DYH.Core.Caching;
using DYH.Models;
using DYH.IDAL;
using DYH.Web.Framework;
using DYH.Web.Framework.Utils;

namespace DYH.Web.Controllers
{
    public class ModulesController : BaseController
    {
        private readonly IModule _module;
        private readonly ICacheManager _cache;
        public ModulesController(IModule module, ICacheManager cache)
        {
            _module = module;
            _cache = cache;
        }

        public ActionResult Index(int id = 0)
        {
            _cache.Remove(Constants.CACHE_KEY_MODULES);
            Utility.GetModelState(this);
            ViewBag.CurrentId = id;

            var list = _cache.Get(Constants.CACHE_KEY_MODULES, () => _module.GetList());
            var model = GetTree(list, id);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult Carte()
        {
            var id = DataCast.Get<int>(ViewBag.CurrentMenuID);
            var list = _cache.Get(Constants.CACHE_KEY_MODULES, () => _module.GetList());
            var model = GetTree(list, id);

            return View(model);
        }

        public static ModuleEntry GetTree(IEnumerable<ModuleEntry> list, int currentId)
        {
            var root = new ModuleEntry
            {
                ModuleCode = "Root",
                DisplayName = "Root",
                SeqNo = 1,
                ParentId = 0,
                ModuleId = 0
            };

            return GetSubItem(list, root, currentId);
        }

        private static ModuleEntry GetSubItem(IEnumerable<ModuleEntry> source, ModuleEntry parentNode, int currentId)
        {
            if (source == null || parentNode == null)
            {
                return null;
            }
            var list = source.Where(x => x.ParentId == parentNode.ModuleId).OrderBy(x => x.SeqNo);

            foreach (var item in list)
            {
                var child = Utils.Dereference(item);
                if (child.ModuleId == currentId)
                {
                    child.IsActived = true;
                }
                else
                {
                    child.IsActived = false;
                }

                parentNode.Children.Add(GetSubItem(source, child, currentId));
                if (child.Children.Any(x => x.IsActived))
                {
                    child.IsActived = true;
                }
            }

            return parentNode;
        }

        public ActionResult Create(int id = 0)
        {
            var info = _module.GetById(id);
            var module = new ModuleEntry
            {
                ParentId = info != null ? id : 0, 
                NonParent = id == 0 ? "Root" : (info != null ? info.DisplayName : "Root")
            };

            return View(module);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(ModuleEntry model)
        {
            var info = _module.GetByCode(model.ModuleCode);

            if (info != null && info.ModuleCode == model.ModuleCode)
            {
                ModelState.AddModelError("ModuleCode", string.Format("{0} has been used, please change one.", "Module code"));
            }

            if (ModelState.IsValid)
            {
                model.CreatedBy = Utility.CurrentUserName;
                model.CreatedTime = DateTime.UtcNow;
                Utility.Operate(this, Operations.Add, () =>
                {
                    _cache.Remove(Constants.CACHE_KEY_MODULES);
                    return _module.Add(model);
                }, model.DisplayName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return Redirect("~/Modules/Index/" + model.ParentId);
        }

        [HttpGet]
        public JsonResult CheckCode(string moduleCode)
        {
            var flag = true;
            var info = _module.GetByCode(moduleCode);
            if (info != null)
            {
                flag = false;
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var list = _cache.Get(Constants.CACHE_KEY_MODULES, () => _module.GetList());
            var info = list.FirstOrDefault(x => x.ModuleId == id);

            if (info != null && info.ParentId == 0)
            {
                info.NonParent = "Root";
            }
            else if(info != null && info.ParentId != 0)
            {
                info.NonParent = list.FirstOrDefault(x => x.ModuleId == info.ParentId).DisplayName;
            }

            return View(info);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(ModuleEntry model)
        {
            if (ModelState.IsValid)
            {
                var list = _cache.Get(Constants.CACHE_KEY_MODULES, () => _module.GetList());
                var info = list.FirstOrDefault(x => x.ModuleId == model.ModuleId);

                if (info != null)
                {
                    model.CreatedBy = info.CreatedBy;
                    model.CreatedTime = info.CreatedTime;
                }

                model.ChangedTime = DateTime.UtcNow;
                model.ChangedBy = Utility.CurrentUserName;

                Utility.Operate(this, Operations.Update, () =>
                {
                    _cache.Remove(Constants.CACHE_KEY_MODULES);
                    return _module.Update(model);
                }, model.DisplayName);
            }
            else
            {
                Utility.SetErrorModelState(this);
            }

            return Redirect("~/Modules/Index/" + model.ModuleId);
        }

        public ActionResult Delete(int id, int parentId = 0)
        {
            Utility.Operate(this, Operations.Delete, () =>
            {
                _cache.Remove(Constants.CACHE_KEY_MODULES);
                return _module.Delete(id);
            });

            return Redirect("~/Modules/Index/" + parentId);
        }
    }
}

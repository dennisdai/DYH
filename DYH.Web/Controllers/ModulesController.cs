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
        private readonly IAction _action;
        private readonly IActionModule _actionModule;
        private readonly ICacheManager _cache;
        public ModulesController(IModule module, IAction action, IActionModule actionModule, ICacheManager cache)
        {
            _module = module;
            _action = action;
            _actionModule = actionModule;
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

            var list = _cache.Get(Constants.CACHE_KEY_ACTIONS, () => _action.GetList());
            ViewBag.Actions = list;

            return View(module);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(ModuleEntry model, FormCollection collection)
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
                    _cache.Remove(Constants.CACHE_KEY_ACTIONMODULE);

                    var moduleId = _module.Add(model);
                    if (moduleId > 0)
                    {
                        var actionModules = SetActionModules(collection, moduleId);
                        _actionModule.Add(actionModules);
                    }

                    return moduleId;

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

            var actions = _cache.Get(Constants.CACHE_KEY_ACTIONS, () => _action.GetList());
            ViewBag.Actions = actions;

            var actionModules = _cache.Get(Constants.CACHE_KEY_ACTIONMODULE, () => _actionModule.GetList());
            ViewBag.ActionModules = actionModules.Where(x => x.ModuleId == id);

            return View(info);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(ModuleEntry model, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var modules = _cache.Get(Constants.CACHE_KEY_MODULES, () => _module.GetList());
                var info = modules.FirstOrDefault(x => x.ModuleId == model.ModuleId);

                if (info != null)
                {
                    model.CreatedBy = info.CreatedBy;
                    model.CreatedTime = info.CreatedTime;
                }

                model.ChangedTime = DateTime.UtcNow;
                model.ChangedBy = Utility.CurrentUserName;

                var actionModules = SetActionModules(collection, model.ModuleId);
                Utility.Operate(this, Operations.Update, () =>
                {
                    _cache.Remove(Constants.CACHE_KEY_MODULES);
                    _cache.Remove(Constants.CACHE_KEY_ACTIONMODULE);

                    var adds = actionModules.Where(x => x.ActionModuleId == 0);
                    if (adds.Any())
                        _actionModule.Add(adds);

                    var updates = actionModules.Where(x => x.ActionModuleId != 0);
                    if (updates.Any())
                        _actionModule.Update(updates);

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

        private List<ActionModuleEntry> SetActionModules(FormCollection collection, int moduleId)
        {
            var actionIDs = collection["ActionModule"];
            var actionModuleIDs = collection["ActionModuleId"];

            var list = new List<string>();
            if (!string.IsNullOrEmpty(actionIDs))
            {
                list = actionIDs.Split(',').ToList();
            }

            var arrIDs = actionModuleIDs.Split(',');

            var actions = _cache.Get(Constants.CACHE_KEY_ACTIONS, () => _action.GetList());

            var actionModules = new List<ActionModuleEntry>();
            for (var i = 0; i < actions.Count(); i++)
            {
                var actionModuleId = DataCast.Get<int>(arrIDs[i]);
                int actionId = actions.ElementAt(i).ActionId;
                var amInfo = new ActionModuleEntry
                {
                    ActionId = actionId,
                    ModuleId = moduleId,
                    Status = list.Any(x => x == actionId.ToString()),
                    ActionModuleId = actionModuleId
                };

                actionModules.Add(amInfo);
            }

            return actionModules;
        }
    }
}

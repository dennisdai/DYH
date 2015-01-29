using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DYH.Core;
using DYH.Core.Caching;
using DYH.IDAL;
using DYH.Models;
using DYH.Web.Framework;
using DYH.Web.Framework.Utils;

namespace DYH.Web.Controllers
{
    public class RoleRightsController : Controller
    {
        private readonly IRole _role;
        private readonly IRoleRight _roleRight;
        private readonly IActionModule _actionModule;
        private readonly IModule _module;
        private readonly IAction _action;
        private readonly ICacheManager _cache;

        public RoleRightsController(IRole role, IModule module, IAction action, IActionModule actionModule, IRoleRight roleRight, ICacheManager cache)
        {
            _role = role;
            _module = module;
            _actionModule = actionModule;
            _roleRight = roleRight;
            _action = action;
            _cache = cache;
        }

        public ActionResult Index(int id = 0, int roleId = 0)
        {
            Utility.GetModelState(this);

            var modules =  _cache.Get(Constants.CACHE_KEY_MODULES, () => _module.GetList());
            var actionModules = _cache.Get(Constants.CACHE_KEY_ACTIONMODULE, () => _actionModule.GetList());
            var roles = _cache.Get(Constants.CACHE_KEY_ROLES, () => _role.GetList());
            var actions = _cache.Get(Constants.CACHE_KEY_ACTIONS, () => _action.GetList());

            roleId = roleId == 0 ? roles.Max(x => x.RoleId) : roleId;
            var roleRights = _roleRight.GetList(roleId);
            var tree = TreeUtils.GetTree(modules, id);

            ViewBag.Module = tree;
            ViewBag.Actions = actions;
            ViewBag.ModuleList = modules;
            ViewBag.CurrentId = id;
            ViewBag.RoleId = roleId;
            ViewBag.RoleRights = roleRights;
            
            ViewBag.Roles = new SelectList(roles, "RoleId", "DisplayName", roleId);

            return View(actionModules);
        }

        public ActionResult Handle(FormCollection collection)
        {
            var actionModules = _cache.Get(Constants.CACHE_KEY_ACTIONMODULE, () => _actionModule.GetList());

            var selectedModuleId = DataCast.Get<int>(collection["SelectedModuleId"]);
            var moduleIDs = collection["ModuleId"];
            var roleId = DataCast.Get<int>(collection["RoleId"]);
            var arrayModuleId = moduleIDs.Split(',');

            var list = new List<RoleRightEntry>();
            foreach (var item in arrayModuleId)
            {
                var moduleId = int.Parse(item);
                var amList = actionModules.Where(x => x.ModuleId == moduleId);
                foreach (var model in amList)
                {
                    var chkName = "ActionModule_" + model.ActionModuleId;
                    var ramIdName = "RightId_" + model.ActionModuleId;

                    var chkVal = collection[chkName];
                    var rightId = DataCast.Get<int>(collection[ramIdName]);

                    var info = new RoleRightEntry
                    {
                        ActionModuleId = model.ActionModuleId,
                        RoleId = roleId,
                        RightId = rightId,
                        Status = chkVal == "on"
                    };

                    list.Add(info);
                }
            }

            var addList = list.Where(x => x.RightId == 0);
            var updateList = list.Where(x => x.RightId != 0);

            Utility.Operate(this, Operations.Save, () =>
            {
                if (addList.Any())
                    _roleRight.Add(addList);
                
                if (updateList.Any())
                    _roleRight.Update(updateList);

                return 1;
            });
           

            return Redirect(string.Format("~/RoleRights/Index/{0}?roleId={1}", selectedModuleId, roleId));
        }
    }
}

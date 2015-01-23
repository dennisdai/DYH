using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DYH.Core.Caching;
using DYH.IDAL;
using DYH.Models;
using DYH.Web.Framework.Utils;

namespace DYH.Web.Controllers
{
    public class RoleRightsController : Controller
    {
        private readonly IRole _role;
        private readonly IRoleRight _roleRight;
        private readonly IActionModule _actionModule;
        private readonly IModule _module;
        private readonly ICacheManager _cache;

        public RoleRightsController(IRole role, IModule module, IActionModule actionModule, IRoleRight roleRight, ICacheManager cache)
        {
            _role = role;
            _module = module;
            _actionModule = actionModule;
            _roleRight = roleRight;
            _cache = cache;
        }

        public ActionResult Index(int id = 0)
        {
            var list =  _cache.Get(Constants.CACHE_KEY_MODULES, () => _module.GetList());
            var tree = TreeUtils.GetTree(list, id);
            ViewBag.Module = tree;
            ViewBag.CurrentId = id;

            return View();
        }

    }
}

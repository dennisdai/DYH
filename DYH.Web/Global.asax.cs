using System.Reflection;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
using DYH.Core.Caching;
using DYH.DAL;
using DYH.Data;
using DYH.IDAL;
using DYH.Web.Controllers;

namespace DYH.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();

            builder.Register(c => new HttpContextWrapper(HttpContext.Current) as HttpContextBase)
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("dyh_cache_static").SingleInstance();
            builder.RegisterType<RequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("dyh_cache_per_request").InstancePerLifetimeScope();
            
            builder.RegisterType<DataProvider>();
            builder.RegisterType<UserRepository>().As<IUser>();
            builder.RegisterType<ActionRepository>().As<IAction>();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ActionsController>().WithParameter(ResolvedParameter.ForNamed<ICacheManager>("dyh_cache_static"));
           
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using DYH.Core.Caching;
using DYH.DAL;
using DYH.Data;
using DYH.IDAL;
using DYH.Web.Controllers;

namespace DYH.Web
{
    public class Resolver
    {
        public static void Init()
        {
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
            builder.RegisterType<ModuleRepository>().As<DYH.IDAL.IModule>();
            builder.RegisterType<RoleRepository>().As<IRole>();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ActionsController>().WithParameter(ResolvedParameter.ForNamed<ICacheManager>("dyh_cache_static"));
            builder.RegisterType<ModulesController>().WithParameter(ResolvedParameter.ForNamed<ICacheManager>("dyh_cache_static"));
            builder.RegisterType<RolesController>().WithParameter(ResolvedParameter.ForNamed<ICacheManager>("dyh_cache_static"));

            var container = builder.Build();

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
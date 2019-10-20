using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using FluentValidation;
using MediatR;
using Triangles.Domain.TriangleFinder;

namespace Triangles.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterAutofac();
        }

        private void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterSource(new ViewRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(FindTriangleByVertices).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.RegisterType<FindTriangleByVertices.Validator>().As<IValidator<FindTriangleByVertices.Query>>().InstancePerLifetimeScope();
            builder.RegisterType<FindTriangleByCoordinates.Validator>().As<IValidator<FindTriangleByCoordinates.Query>>().InstancePerLifetimeScope();

            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            builder.Register<ServiceFactory>(x =>
            {
                var c = x.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
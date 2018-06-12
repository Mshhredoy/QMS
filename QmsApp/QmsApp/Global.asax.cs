using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QmsApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public static class ProjectGlobalProperties
        {

            public const string ProjectName = "QMS";
            public const string ProjectHeading = "QMS";
            public const string CopyRightName = "QMS";
          
            public const string DeveloperName = "Encoders Infotech Ltd.";
            public const string DeveloperWebsite = "http://www.encodersbd.com/";
            public static string LogoPathFevicon = "~/img/logo.png";
            public static string LogoPath = "~/img/logo.png";
            public static string CardPath = "~/img/logo.png";
            public static int DeploymentYear = 2018;
            public static string AppVersion = "1.1";
            public static string AppLastUpdate = "04-06-2018";
        }


    }
}

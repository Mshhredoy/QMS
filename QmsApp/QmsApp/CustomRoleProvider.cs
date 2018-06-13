using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using QmsApp.Controllers;
using QmsApp.Models;

namespace QmsApp
{
    public class CustomRoleProvider:RoleProvider
    {
        private int _cacheTimeoutInMinute = 60;

        public override bool IsUserInRole(string userId, string roleName)
        {
            var userRoles = GetRolesForUser(userId);
            return userRoles.Contains(roleName);
        }


        //public override string[] GetRolesForUser(string username)
        //{
        //    if (!HttpContext.Current.User.Identity.IsAuthenticated)
        //    {
        //        return null;
        //    }
        //    //check cache
        //    //var cacheKey = string.Format("{0_role}", username);
        //    //if (HttpRuntime.Cache[cacheKey] != null)
        //    //{
        //    //    return (string[]) HttpRuntime.Cache[cacheKey];
        //    //}
        //    string[] roles = new string[] { };
        //    using (RfwDbContext db = new RfwDbContext())
        //    {
        //        roles = (from a in db.Roles
        //                 join b in db.Users on a.RoleId equals b.RoleId 
        //                 where b.UserName.Equals(username)
        //                 select a.RoleName).ToArray<string>();
        //        //if (roles.Count() > 0)
        //        //{
        //        //    HttpRuntime.Cache.Insert(cacheKey,roles,null,DateTime.Now.AddMinutes(_cacheTimeoutInMinute), Cache.NoSlidingExpiration);
        //        //}

        //    }
        //    return roles;
        //}

        public override string[] GetRolesForUser(string userid)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return null;
            }
            //check cache
            //var cacheKey = string.Format("{0_role}", username);
            //if (HttpRuntime.Cache[cacheKey] != null)
            //{
            //    return (string[]) HttpRuntime.Cache[cacheKey];
            //}
            string[] RoleTasks = new string[] { };
            int id = Convert.ToInt32(userid.Split('|')[0]);
            using (QmsDbContext db = new QmsDbContext())
            {
                RoleTasks = (from a in db.RoleTasks
                             join b in db.Users on new {a.RoleId } equals new {b.RoleId }
                             where b.UserId.Equals(id)
                             select a.Task).ToArray<string>();
                //var user = db.Users.FirstOrDefault(u => u.UserName == username);
                //deptRoles =
                //    db.DeptRoleConfigs.Where(x => x.DepartmentId == user.DepartmentId && x.RoleId == user.RoleId)
                //        .Select(x => x.DepartmentRole)
                //        .ToArray<string>();
                //if (roles.Count() > 0)
                //{
                //    HttpRuntime.Cache.Insert(cacheKey,roles,null,DateTime.Now.AddMinutes(_cacheTimeoutInMinute), Cache.NoSlidingExpiration);
                //}

            }
            return RoleTasks;
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }

    }
    // Roles authorization=========================================
    public class RolesAttribute : AuthorizeAttribute
    {
        // Multiple roles authorization=========================================

        public RolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
        // Handle Unauthorized Request =========================================

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // The user is not authenticated
                base.HandleUnauthorizedRequest(filterContext);
            }else if (!this.Roles.Split(',').Any(filterContext.HttpContext.User.IsInRole))
            {
                // The user is not in any of the listed roles => 
                // show the unauthorized view
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/403_Unauthorized.cshtml"
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }

         
    }
  // check session timeout===============================================================
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSessionTimeOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (context.Session != null)
            {
                if (context.Session.IsNewSession)
                {
                    string sessionCookie = context.Request.Headers["Cookie"];
                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET&#95;SessionId") >= 0))
                    {
                        FormsAuthentication.SignOut();
                        string redirectTo = "~/User/Login";
                        if (!string.IsNullOrEmpty(context.Request.RawUrl))
                        {
                            redirectTo = string.Format("~/User/Login?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
                        }
                        filterContext.HttpContext.Response.Redirect(redirectTo, true);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }

    // [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckConcurrentLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (context.Session["sessionid"] == null)
                context.Session["sessionid"] = "empty";

            //LoginsController loginsCtlr = new LoginsController();

            //string userId = HttpContext.Current.User.Identity.GetUserId();
            if (LoginsController.IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name, context.Session["sessionid"].ToString()))
            {
                if (!LoginsController.IsUserLoggedOnElsewhere(System.Web.HttpContext.Current.User.Identity.Name, context.Session["sessionid"].ToString()))
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    // if it is being used elsewhere, update all their Logins records to LoggedIn = false, except for your session ID
                    LoginsController.LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name, context.Session["sessionid"].ToString());
                    base.OnActionExecuting(filterContext);
                }
            }
            else
            {
                FormsAuthentication.SignOut();
                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary{{ "controller", "User" },
                                                  { "action", "Login" }
 
                                                 });
            }

        }

    }
    
    public class RoleTaskCheckBoxModel
    {

        public string TaskName { get; set; }
        public string TaskNameDisplay { get; set; }
        public bool IsChecked { get; set; }


        public static List<RoleTaskCheckBoxModel> TaskNames = new List<RoleTaskCheckBoxModel>
        {
            new RoleTaskCheckBoxModel {TaskName="User_Setting", TaskNameDisplay="User Setting"},
            new RoleTaskCheckBoxModel {TaskName="Service_Setting", TaskNameDisplay="Service Setting"},
            new RoleTaskCheckBoxModel {TaskName="Counter_Setting", TaskNameDisplay="Counter Setting"},
          
           
        };

        
    }
}

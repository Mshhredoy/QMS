using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using QmsApp.Models;
using QmsApp.Models.ViewModels;
using LoginViewModel = QmsApp.Models.ViewModels.LoginViewModel;
using RegisterViewModel = QmsApp.Models.ViewModels.RegisterViewModel;


namespace QmsApp.Controllers
{
        [Authorize]
    public class UserController : Controller
    {
            private QmsDbContext db = new QmsDbContext();

        public UserManager<ApplicationUser> UserManager { get; private set; }

        // GET: /User/
        [Roles("Global_SupAdmin,User_Creation")]
        public ActionResult Index()
        {
            var users = db.Users.Where(x => x.Status != 2).OrderBy(x => x.UserName).ToList();
            
            return View(users);
        }
      
        #region login logout area
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
       
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                
                // find user by username first
                //var user = await UserManager.FindByNameAsync(model.UserName);
                var user = db.Users.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower() && u.Status != 0);

                if (user != null)
                {
                    MD5 md5Hash = MD5.Create();
                    string hashPassword =  GetMd5Hash(md5Hash, model.Password);
                    var validCredentials = db.Users.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower() && u.Password.Equals(hashPassword) && u.Status != 0);
                    //var validCredentials = await UserManager.FindAsync(model.UserName, model.Password);
                   if (validCredentials == null)
                    {
                        ModelState.AddModelError("", "Invalid credentials. Please try again.");
                    }
                    else
                    {

                        FormsAuthentication.SetAuthCookie(user.UserId + "|" + user.UserName.ToLower(), model.RememberMe);
                        Session["sessionid"] = System.Web.HttpContext.Current.Session.SessionID;
                      
                        Logins login = new Logins();
                        login.UserId = model.UserName.ToLower();
                        login.SessionId = System.Web.HttpContext.Current.Session.SessionID; 
                        login.LoggedIn = true;
                        login.LoggedInDateTime = DateTime.Now;
                        db.Logins.Add(login);
                        db.SaveChanges();


                       if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Dashboard", "Home");
                        }
                        //await SignInAsync(user, model.RememberMe);

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
                ModelState.Remove("Password");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
        #endregion
        #region user register area
        [Roles("Global_SupAdmin,User_Creation")]
        public ActionResult Register()
        {
            ViewBag.Message = "";
            TempData["RegistrationSuccess"] = "";
            ViewBag.RoleId = new SelectList(db.Roles.Where(r => r.Status == 1).OrderBy(x => x.RoleName), "RoleId", "RoleName");
            return View();
        }

        // POST: /User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel userRegister)
        {
            User user = new User();
            MD5 md5Hash = MD5.Create();
            ViewBag.Message = "";
            if (ModelState.IsValid)
            {

                
                user.UserName = userRegister.UserName.ToLower();
                user.RoleId = userRegister.RoleId;
             
                string hashPassword = GetMd5Hash(md5Hash, userRegister.Password);
                user.Password = hashPassword;
                user.Status = 1;
                user.CreatedBy = Convert.ToInt32(User.Identity.GetUserName().Split('|')[0]);
                user.CreatedDate = DateTime.Now;
                user.LastPassChangeDate = DateTime.Now.Date;
                user.PasswordChangedCount = 0;
                db.Users.Add(user);
                TempData["RegistrationSuccess"] = "New user registration successfully complete! Username and Password sent to user by Email.";
                db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
                return RedirectToAction("index");
            }
            else
            {
                ViewBag.Message = "Something went wrong! please try again";
            }
            ViewBag.RoleId = new SelectList(db.Roles.Where(r => r.RoleId != 1 && r.Status == 1).OrderBy(x => x.RoleName), "RoleId", "RoleName", userRegister.RoleId);

          
            return View(userRegister);
        }
        #endregion
        #region password handling area

        // GET: /User/ChangePassword
        public ActionResult ChangePassword(string userName)
        {
            //ViewBag.PasswordAged = "";
            ChangePsswordViewModel model = new ChangePsswordViewModel();
            if (userName != null)
            {
                model.UserName = userName;
            }
            else
            {
              // MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);

                //model.UserName = currentUser.UserName;
                model.UserName = User.Identity.GetUserName().Split('|')[1];
            }

            //ViewBag.PasswordAged = TempData["PasswordAgedMessage"];
            return View(model);
        }




        [HttpPost]
        public ActionResult ChangePassword(ChangePsswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //ViewBag.PasswordHistryAlert = "";
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;

                if (model.NewPassword == model.OldPassword)
                {
                    changePasswordSucceeded = false;
                }
                else
                {
                    MD5 md5Hash = MD5.Create();
                    string hashOldPassword = GetMd5Hash(md5Hash, model.OldPassword);
                    string hashNewPassword = GetMd5Hash(md5Hash, model.NewPassword);

                    //changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                    var user = db.Users.FirstOrDefault(u => u.UserName.Equals(model.UserName) && u.Password.Equals(hashOldPassword) && u.Status != 0);

                    if (user == null)
                    {
                        return HttpNotFound();
                    }


                    if (hashNewPassword != user.Password)
                    {

                        user.Password = hashNewPassword;
                        user.LastPassChangeDate = DateTime.Now.Date;
                        user.PasswordChangedCount += 1;

                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges(user.UserId.ToString());
                        changePasswordSucceeded = true;

                    }
                    else
                    {
                        //ViewBag.PasswordHistryAlert = "You can not use previously used password!";
                        changePasswordSucceeded = false;
                    }
                }


                if (changePasswordSucceeded)
                {
                    if (!User.Identity.IsAuthenticated)
                    {
                        return RedirectToAction("Login");
                    }
                  

                }
                else
                {

                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");


                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        [Roles("Global_SupAdmin,User_Creation")]

        public ActionResult EditUser(int id)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            var user = db.Users.Find(id);
            model.UserId = user.UserId;
            model.UserName = user.UserName;
            model.RoleId = user.RoleId;
            ViewBag.RoleId = new SelectList(db.Roles.Where(r => r.RoleId != 1 && r.Status == 1).OrderBy(x => x.RoleName), "RoleId", "RoleName", model.RoleId);
            return View(model);
        }


        [HttpPost]
        public ActionResult EditUser(ResetPasswordViewModel model)
        {
            string exMsg = "";
            if (ModelState.IsValid)
            {
               
                try
                {
                    MD5 md5Hash = MD5.Create();
                    string hashNewPassword = GetMd5Hash(md5Hash, model.NewPassword);
                    var user = db.Users.FirstOrDefault(u => u.UserId == model.UserId);

                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    user.UserName = model.UserName;
                    user.Password = hashNewPassword;
                    user.LastPassChangeDate = DateTime.Now;
                    user.PasswordChangedCount += 1;
                    user.UpdatedBy =Convert.ToInt16( User.Identity.GetUserName().Split('|')[0]);
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
                    exMsg = "Password Changed";
                }
                catch (Exception r)
                {
                    exMsg = r.Message;
                }
               

            }
            // If we got this far, something failed, redisplay form
            else
            {
                exMsg = "Model Validation Error";
               
            }
            ViewBag.ExMsg = exMsg;
             return RedirectToAction("Index");

        }
      
        #endregion
        #region User deactivation area
        public ActionResult DeleteUser(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Result = "Error" });
            }
            user.Status = 0;
            db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
            return Json(new { Result = "OK" });
        }
        [Roles("Global_SupAdmin,User_Creation")]
        public ActionResult DeactiveActiveUser(int? id, byte? status)
        {
            if (id == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Result = "Error" });
            }
            user.Status = status;
            db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
            return Json(new { Result = "OK" });
        }
        #endregion
        #region custom user roles area
        //user defined roles =======================================
       [Roles("Global_SupAdmin,Role_Creation")]
        public ActionResult Roles()
        {
           
            return View(db.Roles.Where(x=>x.Status!=2).ToList());
        }
       [Roles("Global_SupAdmin,Role_Creation")]
        public ActionResult CeateRole()
        {
            var model = new RoleViewModel();
            model.RoleTaskCheckBoxList = RoleTaskCheckBoxModel.TaskNames.OrderBy(x => x.TaskNameDisplay).ToList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CeateRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Role role = new Role
                {
                   RoleName = model.RoleName,
                    Status = 1,
                    //CreatedBy = Convert.ToInt32(Membership.GetUser(User.Identity.Name, true).ProviderUserKey),
                    CreatedDate = DateTime.Now,
                };
                role.RoleTasks=new List<RoleTask>();
                if (model.RoleTaskCheckBoxList.Any())
                {
                    List<RoleTaskCheckBoxModel> roleTaskCheckBoxList = model.RoleTaskCheckBoxList.Where(x => x.IsChecked).ToList();
                    foreach (var task in roleTaskCheckBoxList)
                    {
                        var roleTask = new RoleTask
                        {
                            Task = task.TaskName
                        };
                        role.RoleTasks.Add(roleTask);
                    }

                }
                db.Roles.Add(role);
                db.SaveChanges();
                
                return RedirectToAction("Register", "User");
            }
            
            return View(model);
        }
        [Roles("Global_SupAdmin,Role_Creation")]
        public ActionResult EditRole(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.Roles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel model=new RoleViewModel
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Status = role.Status,
            };
            model.RoleTaskCheckBoxList=new List<RoleTaskCheckBoxModel>();
            foreach (var item in RoleTaskCheckBoxModel.TaskNames.OrderBy(x=>x.TaskNameDisplay))
            {
                item.IsChecked = role.RoleTasks.Any(x => x.Task.Equals(item.TaskName));
                model.RoleTaskCheckBoxList.Add(item);
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = db.Roles.Find(model.RoleId);

                role.RoleName = model.RoleName;
                role.Status = model.Status;
                role.UpdatedBy = Convert.ToInt32(User.Identity.GetUserName().Split('|')[0]);
                role.UpdatedDate = DateTime.Now;
 
                var roleTasks = role.RoleTasks.ToList();
                foreach (var removeTask in roleTasks)
                {
                    db.RoleTasks.Remove(removeTask);
                }
                db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
                
                role.RoleTasks = new List<RoleTask>();
                if (model.RoleTaskCheckBoxList.Any())
                {
                    List<RoleTaskCheckBoxModel> roleTaskCheckBoxList = model.RoleTaskCheckBoxList.Where(x => x.IsChecked).ToList();
                    foreach (var task in roleTaskCheckBoxList)
                    {
                        var roleTask = new RoleTask
                        {
                            Task = task.TaskName
                        };
                        role.RoleTasks.Add(roleTask);
                    }

                }
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges(User.Identity.GetUserName().Split('|')[0]);
                return RedirectToAction("Roles", "User");
            }
           
            return View("EditRole", model);
        }
        #endregion

        // Module ============================================================================================================
        #region modules area
        //public ActionResult CheckEmailExits(string Email)
        //{
        //    if (db.Users.Count(e => e.Email == Email) == 0)
        //    {
        //        return Content("true");
        //    }
        //    else
        //    {
        //        return Content("false");
        //    }
        //}

        //public ActionResult CheckPhoneExits(string ContactNo)
        //{
        //    if (db.Users.Count(e => e.ContactNo == ContactNo) == 0)
        //    {
        //        return Content("true");
        //    }
        //    else
        //    {
        //        return Content("false");
        //    }
        //}
        //public ActionResult CheckUserNameExits(string UserName)
        //{
        //    if (db.Users.Count(e => e.UserName == UserName) == 0)
        //    {
        //        return Content("true");
        //    }
        //    else
        //    {
        //        return Content("false");
        //    }
        //}
        public JsonResult CheckUserNameExits(string UserName, string InitialUserName)
        {
            bool isNotExist = true;
            if (UserName != string.Empty && InitialUserName == "undefined")
            {
                var isExist = db.Users.Any(x => x.UserName.ToLower().Equals(UserName.ToLower()));
                if (isExist)
                {
                    isNotExist = false;
                }
            }
            if (UserName != string.Empty && InitialUserName != "undefined")
            {
                var isExist = db.Users.Any(x => x.UserName.ToLower() == UserName.ToLower() && x.UserName.ToLower() != InitialUserName.ToLower());
                if (isExist)
                {
                    isNotExist = false;
                }
            }
            return Json(isNotExist, JsonRequestBehavior.AllowGet);
        }
       
        private async Task<bool> IsFirstLogin(int? userId)
        {
            var user = db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user.PasswordChangedCount > 0 || user.PasswordChangedCount == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }
        }
        public bool CheckPasswordStrength(string userName, string fullName, string password)
        {
            List<string> fullNameParts = fullName.Split(' ').ToList();
            var count = 0;
            if (password.ToLower().Contains(userName.ToLower()))
            {
                count += 1;
            }

            foreach (var namePart in fullNameParts)
            {
                if (password.ToLower().Contains(namePart.ToLower()))
                {
                    count += 1;
                }
            }
            if (count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //name or email exist checker

         [AllowAnonymous]
        public JsonResult CheckUserNameExist(string userName)
        {
            var isUserNameExist = db.Users.Any(u => u.UserName.ToLower() == userName.ToLower());
            if (!isUserNameExist)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        // [AllowAnonymous]
        //public JsonResult CheckEmailExist(string email)
        //{
        //    var isEmailExist = db.Users.Any(u => u.Email.ToLower() == email.ToLower() && u.Status != 0);
        //    if (!isEmailExist)
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(false, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public bool EmailNotExist(string email)
        //{
        //    var isEmailExist = db.Users.Any(u => u.Email.ToLower() == email.ToLower() && u.Status!=0);
        //    if (!isEmailExist)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        // GET: /Account/Manage
        public ActionResult Manage(UserController.ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == UserController.ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == UserController.ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == UserController.ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == UserController.ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }
        public IEnumerable<string> AllPartsOfLength(string value, int length)
        {
            for (int startPos = 0; startPos <= value.Length - length; startPos++)
            {
                yield return value.Substring(startPos, length);
            }
            yield break;
        }

        public bool IsSupUser(string userName)
        {
            if (userName == "lc_admin")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    #region login controller
    public class LoginsController : Controller
    {
        private static QmsDbContext db = new QmsDbContext();
        //
        // GET: /Logins/
        public static bool IsYourLoginStillTrue(string userId, string sid)
        {


            //IEnumerable<Logins> logins = (from i in context.Logins
            //                              where i.LoggedIn == true &&
            //                              i.UserId == userId && i.SessionId == sid
            //                              select i).AsEnumerable();
            var logins = db.Logins.Where(i => i.LoggedIn == true && i.UserId == userId && i.SessionId == sid);
            return logins.Any();
        }

        public static bool IsUserLoggedOnElsewhere(string userId, string sid)
        {


            //IEnumerable<Logins> logins = (from i in context.Logins
            //                              where i.LoggedIn == true &&
            //                              i.UserId == userId && i.SessionId != sid
            //                              select i).AsEnumerable();
            var logins = db.Logins.Where(i => i.LoggedIn == true && i.UserId == userId && i.SessionId != sid);
            return logins.Any();
        }

        public static void LogEveryoneElseOut(string userId, string sid)
        {


            //IEnumerable<Logins> logins = (from i in context.Logins
            //                              where i.LoggedIn == true &&
            //                              i.UserId == userId &&
            //                              i.SessionId != sid // need to filter by user ID
            //                              select i).AsEnumerable();
            var logins = db.Logins.Where(i => i.LoggedIn == true && i.UserId == userId && i.SessionId != sid);

            foreach (Logins item in logins)
            {
                item.LoggedIn = false;
            }

            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    #endregion

    }
   
}

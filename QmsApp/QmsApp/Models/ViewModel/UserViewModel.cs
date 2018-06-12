using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;


namespace QmsApp.Models.ViewModels
{
    [NotMapped]
    public class RegisterViewModel:User
    {
        

        //[Remote("CheckUserNameExist", "User", ErrorMessage = "Username already Exist")]
        //[Editable(true)]
       [Required(ErrorMessage = "Userame field is required")]
        [Display(Name = "User Name")]
        public new string UserName { get; set; }

        [Required(ErrorMessage = "Password field is required")]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        //[RegularExpression(@"^.*(?=^.{6,32}$)((?=.*[a-z])|(?=.*[A-Z]))((?=.*\d)|(?=.*[\W])).*$", ErrorMessage = "Password must be minimum 6 character long and must contain atleast 1 number or special character")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public new string Password { get; set; }

         [Required(ErrorMessage = "Confirm password field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
        ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        //[RegularExpression(@"^.*(?=^.{6,64}$)((?=.*[a-z])|(?=.*[A-Z]))((?=.*\d)(?=.*[\W])).*$", ErrorMessage = "Password must be minimum 6 character long and must contain atleast 1 number and special character")]

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        public int RoleId { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePsswordViewModel
    {
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        //[RegularExpression(@"^.*(?=^.{8,12}$)((?=.*[a-z])|(?=.*[A-Z]))((?=.*\d)|(?=.*[\W])).*$", ErrorMessage = "Password must be 8-12 character long and must contain atleast 1 number or special character")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
    [NotMapped]
    public class RoleViewModel : Role
    {

        public new int? RoleId { get; set; }
        public List<RoleTaskCheckBoxModel> RoleTaskCheckBoxList { get; set; }
    }
}


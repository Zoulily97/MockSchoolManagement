using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse",controller:"Account")]
        [Display(Name ="邮箱地址")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Display(Name ="密码")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name ="确认密码")]
        [Compare("Password",ErrorMessage ="两次密码输入不一致，重新输入")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "居住城市")]
        public string City { get; set; }

    }
}

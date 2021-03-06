﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TopLearn.Core.DTOs
{
    public class UserProfileViewModel
    {
        [Display(Name = "نام و نام خانوادگی")]
        public string Name { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "آواتار")]
        public string Avatar { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegisterDate { get; set; }

        [Display(Name = "موجودی کیف پول")]
        public int WalletBalance { get; set; }
    }

    public class UserSidebarViewModel
    {
        [Display(Name = "نام و نام خانوادگی")]
        public string Name { get; set; }

        [Display(Name = "آواتار")]
        public string Avatar { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegisterDate { get; set; }
    }

    public class EditProfileViewModel
    {
        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        //TODO: Add remote validation
        public string Email { get; set; }

        public IFormFile ImageFile { get; set; }

        [Display(Name = "آواتار")]
        public string ImageName { get; set; }

    }

    public class ChangePasswordViewModel
    {
        [Display(Name = "کلمه عبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string CurrentPassword { get; set; }

        [Display(Name = "کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MinLength(6, ErrorMessage = "{0} باید بیشتر از {1} کاراکتر باشد")]
        public string NewPassword { get; set; }

        [Display(Name = "تکرار کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Compare("NewPassword", ErrorMessage = "{0} با {1} مطابقت ندارد")]
        public string RepeatNewPassword { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.DataLayer.Entities.User
{
    public class User
    {
        public User()
        { }

        public int Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Name { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MinLength(6, ErrorMessage = "{0} باید بیشتر از {1} کاراکتر باشد")]
        public string Password { get; set; }

        [Display(Name = "کد فعال سازی")]
        public string ActivationCode { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        [Display(Name = "آواتار")]
        public string Avatar { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegisterDate { get; set; }

        #region Relations

        public virtual List<UserRole> UserRoles { get; set; }

        public virtual List<Transaction> Transactions { get; set; }

        #endregion
    }
}

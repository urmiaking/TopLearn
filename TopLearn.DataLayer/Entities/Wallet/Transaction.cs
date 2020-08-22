using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TopLearn.DataLayer.Entities.Wallet
{
    public class Transaction
    {
        public Transaction()
        { }

        public int Id { get; set; }

        [Required]
        [Display(Name = "مبلغ تراکنش")]
        public int Amount { get; set; }

        [Display(Name = "وضعیت پرداخت")]
        public bool IsPaid { get; set; }

        [Display(Name = "شرح تراکنش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد")]
        public string Description { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Display(Name = "نوع تراکنش")]
        public TransactionType TransactionType { get; set; }

        [Required]
        public int UserId { get; set; }

        #region Relations

        public virtual User.User User { get; set; }

        #endregion
    }

    public enum TransactionType
    {
        WithDraw, //برداشت
        Deposit   //واریز
    }
}

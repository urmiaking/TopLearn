using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.DTOs
{
    public class ChargeWalletViewModel
    {
        [Display(Name = "مبلغ")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Range(1000, 1000000, ErrorMessage = "{0} می تواند مقداری بین {1} تومان و {2} تومان باشد")]
        public int Amount { get; set; }
    }

    public class WalletViewModel
    {
        [Display(Name = "مبلغ")]
        public int Amount { get; set; }

        [Display(Name = "نوع تراکنش")]
        public TransactionType TransactionType  { get; set; }

        [Display(Name = "شرح تراکنش")]
        public string Description { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime TransactionDateTime { get; set; }
    }

    public class TransactionViewModel
    {
        public List<WalletViewModel> TransactionsList { get; set; }
        public ChargeWalletViewModel ChargeWallet { get; set; }
    }
}

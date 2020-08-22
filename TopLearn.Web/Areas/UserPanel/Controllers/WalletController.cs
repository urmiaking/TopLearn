using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class WalletController : Controller
    {
        private readonly IUserService _userService;

        public WalletController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("[area]/[controller]")]
        public async Task<IActionResult> Index() => View(await _userService.GetUserTransactionViewModelAsync(User.Identity.Name));

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[area]/[controller]")]
        public async Task<IActionResult> Index(TransactionViewModel transactionForm)
        {
            var email = User.Identity.Name;
            var transactionViewModel = await _userService.GetUserTransactionViewModelAsync(email);
            transactionForm.TransactionsList = transactionViewModel.TransactionsList;

            if (!ModelState.IsValid)
            {
                return View(transactionForm);
            }

            await _userService.ChargeUserWallet(email, transactionForm.ChargeWallet.Amount, "شارژ حساب", true);

            //TODO: Online payment and set isPaid to true if payment successful

            var isSucceed = true;

            if (!isSucceed)
            {
                TempData["Error"] = "مشکلی در شارژ کیف پول شما پیش آمد. لطفا بعدا امتحان کنید";
                return View(transactionForm);
            }

            TempData["Success"] = "کیف پول شما با موفقیت شارژ شد";
            return View(transactionForm);
        }
    }
}

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

            var transactionId = await _userService.ChargeUserWallet(email, 
                transactionForm.ChargeWallet.Amount, "شارژ حساب", false);

            var payment = new ZarinpalSandbox.Payment(transactionForm.ChargeWallet.Amount);

            var result = await payment
                .PaymentRequest("شارژ کیف پول",
                    string.Concat(Request.Scheme, "://", Request.Host.ToUriComponent(),
                        $"/OnlinePayment/{transactionId}"), "masoud.xpress@gmail.com", "09905492104");

            if (result.Status is 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + result.Authority);
            }

            await _userService.RemoveFailedTransactionAsync(
                await _userService.GetTransactionByIdAsync(transactionId));

            TempData["Error"] = "پرداخت با خطا مواجه شد";
            return View(transactionForm);
        }

        [Route("[action]/{transactionId}")]
        public async Task<IActionResult> OnlinePayment(string authority, string status, int transactionId = 0)
        {
            if (string.IsNullOrEmpty(status) ||
                string.IsNullOrEmpty(authority))
            {
                return NotFound();
            }

            if (transactionId is 0)
            {
                return NotFound();
            }

            var transaction = await _userService.GetTransactionByIdAsync(transactionId);

            if (transaction is null)
            {
                return NotFound();
            }

            var payment = new ZarinpalSandbox.Payment(transaction.Amount);

            var result = payment.Verification(authority).Result;

            if (!(result.Status is 100))
            {
                await _userService.RemoveFailedTransactionAsync(transaction);

                ViewBag.IsSuccess = false;
                return View();
            }

            await _userService.VerifyTransactionAsync(transaction);

            ViewBag.RefId = result.RefId;
            ViewBag.IsSuccess = true;
            return View();
        }
    }
}

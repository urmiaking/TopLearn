using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        private readonly IPermissionService _permissionService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext db, IPermissionService permissionService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User> LoginUserAsync(LoginViewModel loginForm)
        {
            return await _db.Users.FirstOrDefaultAsync(a =>
                            a.Email.Equals(OptimizeText.OptimizeEmail(loginForm.Email)) &&
                            a.Password.Equals(PasswordHelper.Hash(loginForm.Password)));
        }

        public async Task<bool> ActivateAccountAsync(string activationCode)
        {
            if (string.IsNullOrWhiteSpace(activationCode))
                return false;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.ActivationCode.Equals(activationCode));

            if (user == null || user.IsActive)
                return false;

            user.IsActive = true;
            user.ActivationCode = Generator.GenerationUniqueName();

            return await UpdateUserAsync(user);
        }

        public async Task<User> GetUserByEmailAsync(string email) =>
            await _db.Users.FirstOrDefaultAsync(u => u.Email.Equals(OptimizeText.OptimizeEmail(email)));

        public async Task<bool> IsEmailExistAsync(string email) =>
            await _db.Users.AnyAsync(user =>
                user.Email.Equals(email));

        public async Task<User> GetUserByActivationCodeAsync(string activationCode) =>
            await _db.Users.FirstOrDefaultAsync(u => u.ActivationCode.Equals(activationCode));

        public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.ActivationCode.Equals(resetPasswordViewModel.ActivationCode));

            if (user == null || !user.IsActive)
                return false;

            user.Password = PasswordHelper.Hash(resetPasswordViewModel.Password);
            user.ActivationCode = Generator.GenerationUniqueName();

            return await UpdateUserAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<UserProfileViewModel> GetUserProfileByEmailAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var profile = new UserProfileViewModel()
            {
                Avatar = user.Avatar,
                Email = user.Email,
                Name = user.Name,
                RegisterDate = user.RegisterDate,
                WalletBalance = await GetUserWalletBalanceAsync(email)
            };

            return profile;
        }

        public async Task<UserSidebarViewModel> GetSidebarDataByEmailAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var sidebar = new UserSidebarViewModel()
            {
                Avatar = user.Avatar,
                Name = user.Name,
                RegisterDate = user.RegisterDate
            };

            return sidebar;
        }

        public async Task<EditProfileViewModel> GetEditProfileDataByEmailAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            var profile = new EditProfileViewModel()
            {
                ImageName = user.Avatar,
                Email = user.Email,
                Name = user.Name
            };

            return profile;
        }

        public async Task<bool> EditUserProfileAsync(EditProfileViewModel profile)
        {
            var user = await GetUserByEmailAsync(_httpContextAccessor.HttpContext.User.Identity.Name);

            if (user == null)
            {
                return false;
            }

            if (profile.ImageFile != null)
            {
                if (profile.ImageFile.Length > 1000000)
                {
                    return false;
                }

                if (profile.ImageName != "default-avatar.png")
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot/images/avatars/", user.Avatar);

                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                    else
                    {
                        //TODO: Log error
                        Console.WriteLine($"The image path cannot be found. Path = {oldImagePath}");
                    }
                }

                user.Avatar = Generator.GenerationUniqueName() + Path.GetExtension(profile.ImageFile.FileName);

                var newImagePath = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/images/avatars/", user.Avatar
                );
                await using (var stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await profile.ImageFile.CopyToAsync(stream);
                }

            }

            await DeleteCookieAsync();

            var claims = new ClaimViewModel()
            {
                Email = OptimizeText.OptimizeEmail(profile.Email),
                Name = profile.Name,
                RememberMe = Convert.ToBoolean(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.AuthenticationMethod).Value)
            };

            await CreateCookieAsync(claims);

            user.Name = profile.Name;
            user.Email = OptimizeText.OptimizeEmail(profile.Email);

            await UpdateUserAsync(user);

            return true;
        }

        public async Task CreateCookieAsync(ClaimViewModel user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.AuthenticationMethod, user.RememberMe.ToString())
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties()
            {
                IsPersistent = user.RememberMe,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(43200)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity), authProperties);
        }

        public async Task DeleteCookieAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel passwordForm, string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            var currentHashedPassword = PasswordHelper.Hash(passwordForm.CurrentPassword);

            if (!currentHashedPassword.Equals(user.Password))
            {
                return false;
            }

            user.Password = PasswordHelper.Hash(passwordForm.NewPassword);
            await UpdateUserAsync(user);
            return true;
        }

        public async Task<int> GetUserWalletBalanceAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            var deposits = await _db.Transactions
                .Where(t =>
                    t.UserId.Equals(user.Id) &&
                    t.IsPaid &&
                    t.TransactionType.Equals(TransactionType.Deposit))
                .SumAsync(t => t.Amount);

            var withDraws = await _db.Transactions
                .Where(t =>
                    t.UserId.Equals(user.Id) &&
                    t.IsPaid &&
                    t.TransactionType.Equals(TransactionType.WithDraw))
                .SumAsync(t => t.Amount);

            var balance = deposits - withDraws;

            return balance < 0 ? int.MinValue : balance;
        }

        public async Task<TransactionViewModel> GetUserTransactionViewModelAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            var userTransactions = await _db.Transactions
                .Where(t =>
                    t.UserId.Equals(user.Id) &&
                    t.IsPaid)
                .ToListAsync();

            var walletViewModels = userTransactions.Select(transaction => new WalletViewModel()
            {
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                Description = transaction.Description,
                TransactionDateTime = transaction.TransactionDate
            }).ToList();

            var transactions = new TransactionViewModel()
            {
                ChargeWallet = new ChargeWalletViewModel(),
                TransactionsList = walletViewModels
            };

            return transactions;
        }

        public async Task<int> ChargeUserWallet(string email, int amount, string description, bool isPaid = false)
        {
            var user = await GetUserByEmailAsync(email);

            var transaction = new Transaction()
            {
                Amount = amount,
                IsPaid = isPaid,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Deposit,
                Description = description,
                UserId = user.Id
            };

            return await AddTransactionAsync(transaction);
        }

        public async Task<int> AddTransactionAsync(Transaction transaction)
        {
            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
            return transaction.Id;
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
            => await _db.Transactions.FindAsync(id);

        public async Task VerifyTransactionAsync(Transaction transaction)
        {
            transaction.IsPaid = true;

            _db.Transactions.Update(transaction);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveFailedTransactionAsync(Transaction transaction)
        {
            _db.Remove(transaction);
            await _db.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsers() => await _db.Users.ToListAsync();

        public async Task<User> GetUserByIdAsync(int id) => await _db.Users.FindAsync(id);

        public async Task<EditUserViewModel> GetEditUserViewModelByUserId(int id)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (user is null)
            {
                return null;
            }

            var editUserViewModel = new EditUserViewModel
            {
                Id = id,
                Email = user.Email,
                ImageName = user.Avatar,
                IsActive = user.IsActive,
                Name = user.Name,
                Roles = user.UserRoles.Select(role => role.RoleId).ToList()
            };

            return editUserViewModel;
        }

        public async Task EditUserAsync(EditUserViewModel editUserViewModel)
        {
            var user = await GetUserByIdAsync(editUserViewModel.Id);

            user.Email = OptimizeText.OptimizeEmail(editUserViewModel.Email);
            user.Avatar = editUserViewModel.ImageName;
            user.Name = editUserViewModel.Name;
            user.Password = editUserViewModel.NewPassword is null
                ? user.Password
                : PasswordHelper.Hash(editUserViewModel.NewPassword);
            user.IsActive = editUserViewModel.IsActive;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            var userRoles = await _permissionService.GetUserRolesByUserIdAsync(editUserViewModel.Id);
            await _permissionService.RemoveUserRoleAsync(userRoles);

            foreach (var roleId in editUserViewModel.Roles)
            {
                var userRole = new UserRole
                {
                    RoleId = roleId,
                    UserId = editUserViewModel.Id
                };
                await _permissionService.AddUserRoleAsync(userRole);
            }
        }
    }
}

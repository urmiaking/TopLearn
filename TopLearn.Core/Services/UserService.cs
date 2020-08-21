using System;
using System.Collections.Generic;
using System.IO;
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

namespace TopLearn.Core.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
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
            await _db.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

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
                WalletBalance = 0
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
                Email = profile.Email,
                Name = profile.Name,
                RememberMe = Convert.ToBoolean(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.AuthenticationMethod).Value)
            };

            await CreateCookieAsync(claims);

            user.Name = profile.Name;
            user.Email = profile.Email;

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
    }
}

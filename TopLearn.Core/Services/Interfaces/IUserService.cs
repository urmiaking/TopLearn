﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region User Getter

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByActivationCodeAsync(string activationCode);

        #endregion

        #region Account

        Task<bool> IsEmailExistAsync(string email);

        Task<User> AddUserAsync(User user);

        Task<User> LoginUserAsync(LoginViewModel loginForm);

        Task<bool> ActivateAccountAsync(string activationCode);

        Task<bool> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel);

        Task<bool> UpdateUserAsync(User user);

        #endregion

        #region Cookie

        Task CreateCookieAsync(ClaimViewModel claim);

        Task DeleteCookieAsync();

        #endregion

        #region User Panel

        Task<UserProfileViewModel> GetUserProfileByEmailAsync(string email);

        Task<UserSidebarViewModel> GetSidebarDataByEmailAsync(string email);

        Task<EditProfileViewModel> GetEditProfileDataByEmailAsync(string email);

        Task<bool> EditUserProfileAsync(EditProfileViewModel profile);

        Task<bool> ChangePasswordAsync(ChangePasswordViewModel passwordForm, string email);

        #endregion
    }
}

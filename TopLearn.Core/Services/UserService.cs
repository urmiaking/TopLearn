﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

        public UserService(AppDbContext db)
        {
            _db = db;
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
    }
}

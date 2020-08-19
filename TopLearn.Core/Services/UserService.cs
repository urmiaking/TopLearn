using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
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

        public async Task<User> LoginUserAsync(LoginViewModel loginForm) =>
            await _db.Users.FirstOrDefaultAsync(a =>
                a.Email.Equals(OptimizeText.OptimizeEmail(loginForm.Email)) &&
                a.Password.Equals(PasswordHelper.Hash(loginForm.Password)));

        public async Task<bool> IsEmailExistAsync(string email) =>
            await _db.Users.AnyAsync(user =>
                user.Email.Equals(email));
    }
}

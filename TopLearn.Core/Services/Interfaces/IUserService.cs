using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> IsEmailExistAsync(string email);

        Task<User> AddUserAsync(User user);

        Task<User> LoginUserAsync(LoginViewModel loginForm);

        Task<bool> ActivateAccountAsync(string activationCode);
    }
}

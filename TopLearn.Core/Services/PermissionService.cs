using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _db;

        public PermissionService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Role>> GetRolesAsync() => await _db.Roles.ToListAsync();

        public async Task<Role> GetRoleByIdAsync(int id) => await _db.Roles.FindAsync(id);

        public async Task<List<UserRole>> GetUserRolesByUserIdAsync(int id) =>
            await _db.UserRoles.Where(a => a.UserId.Equals(id)).ToListAsync();

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _db.AddAsync(userRole);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserRoleAsync(List<UserRole> userRoles)
        {
            if (userRoles.Any())
            {
                _db.UserRoles.RemoveRange(userRoles);
                await _db.SaveChangesAsync();
            }
            
        }
    }
}
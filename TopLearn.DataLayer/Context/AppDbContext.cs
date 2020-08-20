using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TopLearn.DataLayer.Entities.Mail;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Extensions;

namespace TopLearn.DataLayer.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        #region User

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        #endregion

        #region Mail

        public virtual DbSet<MailServer> MailServers { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}

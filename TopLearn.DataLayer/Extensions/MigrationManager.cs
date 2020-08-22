using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TopLearn.DataLayer.Entities.Mail;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.DataLayer.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
        {
            using var scope = host.Services.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<T>();
            try
            {
                appContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return host;
        }

        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MailServer>()
                .HasData(new MailServer()
                {
                    Id = 1,
                    ServerAddress = "masoud.xpress@gmail.com",
                    Password = "MASOUD7559",
                    Port = 587,
                    Host = "smtp.gmail.com"
                });

            modelBuilder.Entity<User>()
                .HasData(new User()
                {
                    Id = 1,
                    ActivationCode = string.Empty,
                    Avatar = "default-avatar.png",
                    Email = "masoud.brilliant@hotmail.com",
                    IsActive = true,
                    Name = "مسعود خدادادی",
                    Password = "db901737c41e490dec8bded913f112e5e7c720c3847558f0e5c65128bdb1b34c",
                    RegisterDate = DateTime.Now
                });

            modelBuilder.Entity<Transaction>()
                .HasData(new Transaction()
                {
                    Id = 1,
                    Amount = 25000,
                    Description = "شارژ حساب",
                    IsPaid = true,
                    TransactionDate = DateTime.Now,
                    TransactionType = TransactionType.Deposit,
                    UserId = 1
                }, new Transaction()
                {
                    Id = 2,
                    Amount = 6000,
                    Description = "شارژ حساب",
                    IsPaid = true,
                    TransactionDate = DateTime.Now,
                    TransactionType = TransactionType.Deposit,
                    UserId = 1
                }, new Transaction()
                {
                    Id = 3,
                    Amount = 8000,
                    Description = "خرید آموزش",
                    IsPaid = true,
                    TransactionDate = DateTime.Now,
                    TransactionType = TransactionType.WithDraw,
                    UserId = 1
                });
        }
    }
}

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
        }
    }
}

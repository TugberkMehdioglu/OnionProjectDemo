using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.ENTITIES.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.ContextClasses
{
    public class MyContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.Load("Project.MAP"));
            base.OnModelCreating(builder);

            //DataSeed for AppUser
            SeedUser(builder);
            SeedRole(builder);
            SeedUserRole(builder);
        }

        public DbSet<AppUser>? AppUsers { get; set; }
        public DbSet<AppUserProfile>? AppUserProfiles { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Order>? Order { get; set; }
        public DbSet<OrderDetail>? OrderDetail { get; set; }
        public DbSet<Product>? Products { get; set; }


        private void SeedUser(ModelBuilder builder)
        {
            AppUser user = new()
            {
                Id = "5c8defd5-91f2-4256-9f16-e7fa7546dec4",
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "Admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "05316453125",
                LockoutEnabled = true,
                CreatedDate = DateTime.Now,
                Status = ENTITIES.Enums.DataStatus.Inserted
            };

            PasswordHasher<AppUser> hasher = new PasswordHasher<AppUser>();
            user.PasswordHash = hasher.HashPassword(user, "Password12*");

            builder.Entity<AppUser>().HasData(user);
        }

        private void SeedRole(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(new IdentityRole()
            {
                Id = "4d7b3bc1-f3aa-48ce-b587-5e7dc5557634",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
        }

        private void SeedUserRole(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>()
            {
                UserId = "5c8defd5-91f2-4256-9f16-e7fa7546dec4",
                RoleId = "4d7b3bc1-f3aa-48ce-b587-5e7dc5557634"
            });
        }
    }
}

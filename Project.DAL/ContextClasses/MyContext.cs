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
        }

        public DbSet<AppUser>? AppUsers { get; set; }
        public DbSet<AppUserProfile>? AppUserProfiles { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Order>? Order { get; set; }
        public DbSet<OrderDetail>? OrderDetail { get; set; }
        public DbSet<Product>? Products { get; set; }
    }
}

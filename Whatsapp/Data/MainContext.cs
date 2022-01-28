
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Whatsapp.Data.MainModels;

namespace Whatsapp.Data
{
    public class MainContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        public MainContext(DbContextOptions<MainContext> options)
               : base(options)
        { }

        public MainContext() { }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Chatbot> Chatbots { get; set; }
        public DbSet<Sessao> Sessoes { get; set; }
        public DbSet<Opcao> Opcoes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<MainModels.Whatsapp> Whatsapps { get; set; }
        public DbSet<ConfigMKAUTH> ConfigsMKAUTH { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=107.178.218.58\SQLEXPRESS,1433;Initial Catalog=whitezap;User ID=whitezap;Password=sad*&H32i");



        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Usuario>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }
    }
}

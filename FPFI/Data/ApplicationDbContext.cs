using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FPFI.Models;
using Microsoft.AspNetCore.Identity;

namespace FPFI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<ApplicationRole>()
                .HasMany(e => e.Users)
                .WithOne()
                .HasForeignKey(e => e.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaims { get; set; }

        public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        public DbSet<Entry2> Entry2 { get; set; }

        public DbSet<Input2> Input2 { get; set; }

        public DbSet<Entry3> Entry3 { get; set; }

        public DbSet<Input3> Input3 { get; set; }

        public DbSet<Product2> Product2 { get; set; }

        public DbSet<Product3> Product3 { get; set; }

        public DbSet<Parameter2> Parameter2 { get; set; }

        public DbSet<Parameter3> Parameter3 { get; set; }

        public DbSet<Simulation2> Simulation2 { get; set; }

        public DbSet<Simulation3> Simulation3 { get; set; }

        public DbSet<Taper2> Taper2 { get; set; }

        public DbSet<Diam> Diams { get; set; }

        public DbSet<Region> Region { get; set; }

        public DbSet<Tree> Tree { get; set; }

        public DbSet<Species> Species { get; set; }
    }
}

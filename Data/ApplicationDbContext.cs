using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Models;

namespace ShiftSchedularApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftDetail> ShiftDetails { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<PayStub> PayStubs { get; set; }
        public DbSet<Punch> Punches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Add foreign key constraints manually, linking EmployeeId to AspNetUsers.Id (Identity)
            builder.Entity<Shift>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Availability>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PayStub>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Punch>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>()
                .WithMany()
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship between Shift and ShiftDetail
            builder.Entity<ShiftDetail>()
                .HasOne(sd => sd.Shift)
                .WithMany(s => s.ShiftDetails)
                .HasForeignKey(sd => sd.ShiftId)
                .OnDelete(DeleteBehavior.Cascade); // If shift is deleted, delete all details
        }

        public static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            
            var defaultUser = await userManager.FindByEmailAsync("dario@gc.ca");
            if (defaultUser == null)
            {
                defaultUser = new IdentityUser
                {
                    UserName = "dario@gc.ca",
                    Email = "dario@gc.ca",
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(defaultUser, "Test123$");
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create default user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}

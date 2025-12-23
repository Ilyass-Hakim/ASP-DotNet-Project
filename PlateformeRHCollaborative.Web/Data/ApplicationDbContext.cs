using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Leave> Leaves => Set<Leave>();
    public DbSet<Telework> Teleworks => Set<Telework>();
    public DbSet<DocumentRequest> DocumentRequests => Set<DocumentRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Hierarchy
        builder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany() // One manager has many employees, but we don't strictly need a collection on the Manager side yet
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

        builder.Entity<Employee>()
            .Property(e => e.SalaryBase)
            .HasPrecision(18, 2);
    }
}

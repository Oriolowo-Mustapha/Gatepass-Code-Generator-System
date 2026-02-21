using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Added back for PostgreSQL support
        static ApplicationDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<GatepassRequest> GatepassRequests { get; set; }
        public DbSet<Gatepass> Gatepasses { get; set; }
        public DbSet<VehicleDetails> VehicleDetails { get; set; }
        public DbSet<CheckInOut> CheckInOuts { get; set; }
        public DbSet<AccessPoint> AccessPoints { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GatepassRequest>()
                .HasOne(gr => gr.Visitor)
                .WithMany(v => v.GatepassRequests)
                .HasForeignKey(gr => gr.VisitorsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GatepassRequest>()
                .HasOne(gr => gr.DestinationDepartment)
                .WithMany(d => d.GatepassRequests)
                .HasForeignKey(gr => gr.DestinationDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GatepassRequest>()
                .HasOne(gr => gr.HostUser)
                .WithMany()
                .HasForeignKey(gr => gr.HostUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GatepassRequest>()
                .HasOne(gr => gr.Approver)
                .WithMany()
                .HasForeignKey(gr => gr.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleDetails>()
                .HasOne(vd => vd.GatepassRequest)
                .WithOne(gr => gr.VehicleDetails)
                .HasForeignKey<VehicleDetails>(vd => vd.GatePassRequestID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gatepass>()
                .HasOne(g => g.GatepassRequest)
                .WithOne(gr => gr.Gatepass)
                .HasForeignKey<Gatepass>(g => g.GatePassRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Visitor>()
                .HasOne(v => v.BlackListedByUser)
                .WithMany()
                .HasForeignKey(v => v.BlackListedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Gatepass>()
                .HasOne(g => g.RevokedBy)
                .WithMany()
                .HasForeignKey(g => g.RevokedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gatepass>()
                .HasIndex(g => g.UniqueCode)
                .IsUnique();

            modelBuilder.Entity<CheckInOut>()
                .HasOne(c => c.Gatepass)
                .WithMany(g => g.CheckInOuts)
                .HasForeignKey(c => c.GatePassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CheckInOut>()
                .HasOne(c => c.CheckInAccessPoint)
                .WithMany()
                .HasForeignKey(c => c.CheckInAccessPointId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckInOut>()
                .HasOne(c => c.CheckOutAccessPoint)
                .WithMany()
                .HasForeignKey(c => c.CheckOutAccessPointId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckInOut>()
                .HasOne(c => c.CheckInPersonnel)
                .WithMany()
                .HasForeignKey(c => c.CheckInPersonnelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckInOut>()
                .HasOne(c => c.CheckOutPersonnel)
                .WithMany()
                .HasForeignKey(c => c.CheckOutPersonnelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
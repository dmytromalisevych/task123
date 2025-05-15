using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HospitalAppointmentSystem.Core.Models;

namespace HospitalAppointmentSystem.Core.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {   
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=HospitalAppointmentSystem.db");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable(name: "Users");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
            
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasIndex(d => d.Email)
                      .IsUnique();
                
                entity.HasOne<IdentityUser>()
                      .WithOne()
                      .HasForeignKey<Doctor>(d => d.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasIndex(p => p.Email)
                      .IsUnique();
                
                entity.HasOne<IdentityUser>()
                      .WithOne()
                      .HasForeignKey<Patient>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasOne(mr => mr.Patient)
                      .WithMany(p => p.MedicalRecords)
                      .HasForeignKey(mr => mr.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(mr => mr.Doctor)
                      .WithMany(d => d.MedicalRecords)
                      .HasForeignKey(mr => mr.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasOne(a => a.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(a => a.DoctorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
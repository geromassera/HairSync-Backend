using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Branch> Branches { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(u => u.Surname)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasConversion<string>()
                    .HasDefaultValue(UserRole.Client);

                entity.Property(u => u.Phone)
                    .HasMaxLength(20);

                // relacion con branch
                entity.HasOne(u => u.Branch)
                  .WithMany()
                  .HasForeignKey(u => u.BranchId)
                  .OnDelete(DeleteBehavior.SetNull);

            });


            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.AppointmentId);

                entity.Property(a => a.AppointmentTime)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(a => a.AppointmentDate)
                    .IsRequired();

                entity.Property(a => a.CreatedAt)
                    .IsRequired();

                entity.HasOne(a => a.Client)
                    .WithMany()
                    .HasForeignKey(a => a.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Barber)
                    .WithMany()
                    .HasForeignKey(a => a.BarberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Treatment>(entity =>
            {
                entity.HasKey(t => t.TreatmentId);

                entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

                entity.Property(t => t.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

                entity.HasData(
                    new Treatment
                    {
                        TreatmentId = 1,
                        Name = "Corte",
                        Price = 4500.00m
                    },
                    new Treatment
                    {
                        TreatmentId = 2,
                        Name = "Corte y Barba",
                        Price = 6500.00m
                    },
                    new Treatment
                    {
                        TreatmentId = 3,
                        Name = "Peinado",
                        Price = 3000.00m
                    },
                    new Treatment
                    {
                        TreatmentId = 4,
                        Name = "Coloración",
                        Price = 8000.00m
                    },
                    new Treatment
                    {
                        TreatmentId = 5,
                        Name = "Barba",
                        Price = 2500.00m
                    }
                );
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.ReviewId);

                entity.Property(r => r.Comment)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(r => r.Rating)
                    .IsRequired();

                // 1 review por turno
                entity.HasIndex(r => r.AppointmentId).IsUnique();

                entity.HasOne(r => r.Appointment)
                      .WithOne(a => a.Review)
                      .HasForeignKey<Review>(r => r.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.BranchId);

                entity.Property(b => b.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(b => b.Address)
                      .IsRequired()
                      .HasMaxLength(200);

                // Las 2 sucursales fijas
                entity.HasData(
                    new Branch 
                    {
                        BranchId = 1, 
                        Name = "Sucursal Centro", 
                        Address = "Av. Pellegrini 1234" 
                    },
                    new Branch 
                    { 
                        BranchId = 2, 
                        Name = "Sucursal Norte",
                        Address = "Bv. Rondeau 4567"
                    }
                );
            });
        }
    }
}

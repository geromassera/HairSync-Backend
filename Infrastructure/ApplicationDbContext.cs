﻿using System;
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
        public DbSet<Treatment> Treatments { get; set; }

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
        }
    }
}

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
                entity.HasKey(a => a.Id);
                // Convertir el Enum a String en la DB (más legible)
                entity.Property(e => e.Status)
                      .IsRequired()
                      .HasConversion<string>()
                      .HasMaxLength(20);


                // Hacer 'AppointmentDateTime' requerida (aunque ya lo es por [Required])
                entity.Property(a => a.AppointmentDateTime)
                    .IsRequired();

                // Configurar la relación con Cliente (User)
                entity.HasOne(a => a.Client)
                      .WithMany() // Asumimos que User no tiene una colección "ClientAppointments"
                      .HasForeignKey(a => a.ClientId)
                      .OnDelete(DeleteBehavior.Restrict); // Evitar borrado en cascada

                // Configurar la relación con Barbero (User)
                entity.HasOne(a => a.Barber)
                      .WithMany() // Asumimos que User no tiene "BarberAppointments"
                      .HasForeignKey(a => a.BarberId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Branch
                entity.HasOne(a => a.Branch)
                      .WithMany() // Asumimos que Branch no tiene "Appointments"
                      .HasForeignKey(a => a.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Treatment
                entity.HasOne(a => a.Treatment)
                      .WithMany() // Asumimos que Treatment no tiene "Appointments"
                      .HasForeignKey(a => a.TreatmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Índice para buscar rápido los turnos de un Cliente
                entity.HasIndex(a => a.ClientId)
                    .HasDatabaseName("IX_Appointment_ClientId");

                // Índice para buscar rápido los turnos de una Sucursal
                entity.HasIndex(a => a.BranchId)
                    .HasDatabaseName("IX_Appointment_BranchId");

                // Índice COMPUESTO: La joya de la corona.
                // Lo usaremos para chequear disponibilidad (BarberId + Fecha)
                // y para buscar la agenda del barbero.
                entity.HasIndex(a => new { a.BarberId, a.AppointmentDateTime })
                    .HasDatabaseName("IX_Appointment_BarberId_DateTime");
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

            modelBuilder.Entity<Review>()
            .HasOne(r => r.User)          // Una Review tiene un User
            .WithMany(u => u.Reviews)    // Un User tiene muchas Reviews
            .HasForeignKey(r => r.UserId) // La clave foránea es UserId
            .OnDelete(DeleteBehavior.Cascade); // Opcional: si borrás un usuario, se borran sus reviews. 
                                               // Podés cambiarlo a .Restrict si preferís que no se pueda borrar un user con reviews.
      
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

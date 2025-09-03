using BitsMVCProject.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BitsMVCProject.Data
{
    public class MVCDbContext:DbContext
    {
        public DbSet<ContactModel> Contacts { get; set; }
        public MVCDbContext(DbContextOptions<MVCDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var contact = modelBuilder.Entity<ContactModel>();

            contact.ToTable("Contacts");

            contact.HasKey(c => c.Id);

            contact.Property(c => c.Id).ValueGeneratedOnAdd();

            contact.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            contact.Property(c => c.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");

            contact.Property(c => c.Married)
                .IsRequired();

            contact.Property(c => c.Phone)
                .IsRequired()
                .HasMaxLength(50);

            contact.Property(c => c.Salary)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        }
    }
}

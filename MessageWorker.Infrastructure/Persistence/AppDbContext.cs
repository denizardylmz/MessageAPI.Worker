using MessageWorker.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<ShiftEntity> Shifts => Set<ShiftEntity>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {


            var utc = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableUtc = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);


            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ShiftEntity>(entity =>
            {
                entity.ToTable("shifts");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.StartTime).HasColumnName("start_time");
                entity.Property(x => x.EndTime).HasColumnName("end_time");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");
                entity.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                entity.Property(x => x.UserId).HasColumnName("user_id");

                entity.Property(x => x.StartTime).HasConversion(utc);
                entity.Property(x => x.CreatedAt).HasConversion(utc);
                entity.Property(x => x.EndTime).HasConversion(nullableUtc);
                entity.Property(x => x.UpdatedAt).HasConversion(nullableUtc);
            });
        }

    }

}

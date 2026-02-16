using MessageWorker.Domain.Entities;
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
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

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

            modelBuilder.Entity<OutboxMessage>(b =>
            {
                b.ToTable("outbox_messages");
                b.HasKey(x => x.Id);

                b.Property(x => x.Id).HasColumnName("id");

                b.Property(x => x.OccurredOnUtc).HasColumnName("occurred_on_utc");
                b.Property(x => x.CreatedOnUtc).HasColumnName("created_on_utc");

                b.Property(x => x.Type).HasColumnName("type").HasMaxLength(200);
                b.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb");

                b.Property(x => x.Status).HasColumnName("status").HasConversion<int>();

                b.Property(x => x.LockedBy).HasColumnName("locked_by").HasMaxLength(200);
                b.Property(x => x.LockUntilUtc).HasColumnName("lock_until_utc");

                b.Property(x => x.TryCount).HasColumnName("try_count");
                b.Property(x => x.LastError).HasColumnName("last_error");

                b.Property(x => x.ProcessedOnUtc).HasColumnName("processed_on_utc");

                b.HasIndex(x => new { x.Status, x.LockUntilUtc, x.CreatedOnUtc });
                b.HasIndex(x => x.ProcessedOnUtc);
            });
        }

    }

}

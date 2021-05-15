using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conduit.Models;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Data
{
    public partial class ConduitContext : DbContext
    {
        public ConduitContext(DbContextOptions<ConduitContext> opt) : base(opt)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public DbSet<Article> Article { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                DateTime now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    entity.Property("CreatedAt").CurrentValue = now;
                }
                entity.Property("UpdatedAt").CurrentValue = now;
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

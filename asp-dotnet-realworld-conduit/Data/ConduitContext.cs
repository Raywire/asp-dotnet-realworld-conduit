using System;
using Conduit.Models;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Data
{
    public partial class ConduitContext : DbContext
    {
        public ConduitContext(DbContextOptions<ConduitContext> opt) : base(opt)
        {

        }

        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

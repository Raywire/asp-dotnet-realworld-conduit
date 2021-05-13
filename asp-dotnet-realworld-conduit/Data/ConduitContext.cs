using System;
using asp_dotnet_realworld_conduit.Models;
using Microsoft.EntityFrameworkCore;

namespace asp_dotnet_realworld_conduit.Data
{
    public partial class ConduitContext : DbContext
    {
        public ConduitContext(DbContextOptions<ConduitContext> opt) : base(opt)
        {

        }

        public virtual DbSet<User> Users { get; set; }

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

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

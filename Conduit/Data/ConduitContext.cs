using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conduit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Conduit.Data
{
    public partial class ConduitContext : DbContext
    {
        public ConduitContext(DbContextOptions<ConduitContext> opt) : base(opt)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Follow> Follows { get; set; }
        public virtual DbSet<Tag> Tags { get; set; } 
        public virtual DbSet<ArticleTag> ArticleTags { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Follow>(b =>
            {
                b.HasKey(t => new { t.FollowerId, t.FollowingId });

                // we need to add OnDelete RESTRICT otherwise for the SqlServer database provider, 
                // app.ApplicationServices.GetRequiredService<ConduitContext>().Database.EnsureCreated(); throws the following error:
                // System.Data.SqlClient.SqlException
                // HResult = 0x80131904
                // Message = Introducing FOREIGN KEY constraint 'FK_FollowedPeople_Persons_TargetId' on table 'FollowedPeople' may cause cycles or multiple cascade paths.Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
                // Could not create constraint or index. See previous errors.
                b.HasOne(pt => pt.Following)
                    .WithMany(p => p.Followers)
                    .HasForeignKey(pt => pt.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict);

                // we need to add OnDelete RESTRICT otherwise for the SqlServer database provider, 
                // app.ApplicationServices.GetRequiredService<ConduitContext>().Database.EnsureCreated(); throws the following error:
                // System.Data.SqlClient.SqlException
                // HResult = 0x80131904
                // Message = Introducing FOREIGN KEY constraint 'FK_FollowingPeople_Persons_TargetId' on table 'FollowedPeople' may cause cycles or multiple cascade paths.Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
                // Could not create constraint or index. See previous errors.
                b.HasOne(pt => pt.Follower)
                    .WithMany(t => t.Following)
                    .HasForeignKey(pt => pt.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ArticleTag>().HasKey(at => new { at.ArticleId, at.TagId });

            OnModelCreatingPartial(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            SetTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetTimestamps()
        {
            IEnumerable<EntityEntry> entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (EntityEntry entity in entities)
            {
                DateTime now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SamuraiApp.Data
{
    public partial class SamuraiAppDataContext : DbContext
    {
        public SamuraiAppDataContext()
        {
        }

        public SamuraiAppDataContext(DbContextOptions<SamuraiAppDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Clans> Clans { get; set; }
        public virtual DbSet<Quotes> Quotes { get; set; }
        public virtual DbSet<Samurais> Samurais { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog = SamuraiAppData");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quotes>(entity =>
            {
                entity.HasIndex(e => e.SamuraiId);

                entity.HasOne(d => d.Samurai)
                    .WithMany(p => p.Quotes)
                    .HasForeignKey(d => d.SamuraiId);
            });

            modelBuilder.Entity<Samurais>(entity =>
            {
                entity.HasIndex(e => e.ClanId);

                entity.HasOne(d => d.Clan)
                    .WithMany(p => p.Samurais)
                    .HasForeignKey(d => d.ClanId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

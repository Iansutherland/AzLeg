using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AzLeg.ConsoleUI.Entities
{
    public partial class AzLegContext : DbContext
    {
        public AzLegContext()
        {
        }

        public AzLegContext(DbContextOptions<AzLegContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article2Article> Article2Articles { get; set; }
        public virtual DbSet<LegArticle> LegArticles { get; set; }
        public virtual DbSet<LegChapter> LegChapters { get; set; }
        public virtual DbSet<LegTitle> LegTitles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=cybertron\\sqlexpress;Database=AzLeg;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Article2Article>(entity =>
            {
                entity.ToTable("Article2Article");

                entity.HasOne(d => d.FromArticleNavigation)
                    .WithMany(p => p.Article2ArticleFromArticleNavigations)
                    .HasForeignKey(d => d.FromArticle)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Article2A__FromA__3F466844");

                entity.HasOne(d => d.ToArticleNavigation)
                    .WithMany(p => p.Article2ArticleToArticleNavigations)
                    .HasForeignKey(d => d.ToArticle)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Article2A__ToArt__403A8C7D");
            });

            modelBuilder.Entity<LegArticle>(entity =>
            {
                entity.ToTable("legArticle");

                entity.Property(e => e.Citation)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UrlAzLeg)
                    .HasMaxLength(2083)
                    .IsUnicode(false);

                entity.HasOne(d => d.Chapter)
                    .WithMany(p => p.LegArticles)
                    .HasForeignKey(d => d.ChapterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__legArticl__Chapt__3C69FB99");

                entity.HasOne(d => d.Title)
                    .WithMany(p => p.LegArticles)
                    .HasForeignKey(d => d.TitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__legArticl__Title__3B75D760");
            });

            modelBuilder.Entity<LegChapter>(entity =>
            {
                entity.ToTable("legChapter");

                entity.HasOne(d => d.Title)
                    .WithMany(p => p.LegChapters)
                    .HasForeignKey(d => d.TitleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__legChapte__Title__38996AB5");
            });

            modelBuilder.Entity<LegTitle>(entity =>
            {
                entity.ToTable("legTitle");

                entity.Property(e => e.Heading)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UrlAzLeg)
                    .HasMaxLength(2083)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

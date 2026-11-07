using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public AppDbContext()
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=GameLibrary;Username=postgres;Password=Bill0405h/");
        }
        public DbSet<GameGenres> GameGenres { get; set; }
        public DbSet<GamePlatforms> GamePlatforms { get; set; }
        public DbSet<Games> Games { get; set; }
        public DbSet<GameTags> GameTags { get; set; }
        public DbSet<Genres> Genres { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Platforms> Platforms { get; set; }
        public DbSet<PlaySessions> PlaySessions { get; set; }
        public DbSet<PlayStatuses> PlayStatuses { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GamePlatforms>()
                .HasKey(x => new { x.Game_Id, x.Platform_Id });

            modelBuilder.Entity<GameGenres>()
                .HasKey(x => new { x.Game_Id, x.Genre_Id });

            modelBuilder.Entity<GameTags>()
                .HasKey(x => new { x.Game_Id, x.Tag_Id });
        }
    }
}

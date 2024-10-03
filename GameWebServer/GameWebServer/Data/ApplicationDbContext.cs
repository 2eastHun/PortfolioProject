using GameWebServer.Models;
using Microsoft.EntityFrameworkCore;

namespace GameWebServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<PlayerData> playerDatas { get; set; }
        public DbSet<RoomData> roomDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerData>().ToTable("PlayerData");
            modelBuilder.Entity<RoomData>().ToTable("RoomData");
        }
    }
}

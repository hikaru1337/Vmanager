using Microsoft.EntityFrameworkCore;
using VManager.db.Model;

namespace DarlingDb
{
    public class Db : DbContext
    {
        private readonly string ConnectionString = @"Data Source = Vmanager.db";
        public DbSet<Vtuber> Vtuber { get; set; }
        public DbSet<Dates> Dates { get; set; }
        public DbSet<Generate> Generate { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Generate>().HasData(
            new Generate[]
            {
                new Generate{ Id = 1, img = new byte[0] }
            });
            
            modelBuilder.Entity<Vtuber>().HasData(
            new Vtuber[]
            {
                new Vtuber{ Id = 1,Name = "KraNf",TelegramId = 493373972,TwitchId = 808276081,Color = "c8738b" },
                new Vtuber{ Id = 2,Name = "Tsunya",TelegramId = 545498845,TwitchId = 118662043, Color = "cb2b72" },
                new Vtuber{ Id = 3,Name = "Pewa",TelegramId = 532671775,TwitchId = 827228470, Color = "ad77ca" },
                new Vtuber{ Id = 4,Name = "Aya",TelegramId = 930000855,TwitchId = 547036080, Color = "b4323d" },
                new Vtuber{ Id = 5,Name = "xXxpososu",TelegramId = 818767715,TwitchId = 820065100, Color = "e8bd5e" },
            });

            
        }
    }

}

using Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class AppDbContext : DbContext
    {
        public DbSet<DataRecord> DataRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Настройте строку подключения
            optionsBuilder.UseNpgsql("Host=localhost;Database=crypto_db;Username=postgres;Password=password");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataRecord>().ToTable("DataRecords");

            modelBuilder.Entity<DataRecord>()
                .Property(d => d.Date)
                .HasConversion(
                    v => ((DateTimeOffset)v).ToUnixTimeSeconds(), // Преобразуем DateTime в Unix timestamp (long) для хранения
                    v => DateTimeOffset.FromUnixTimeSeconds(v).UtcDateTime // Преобразуем Unix timestamp (long) обратно в DateTime
                );
        }

    }

}

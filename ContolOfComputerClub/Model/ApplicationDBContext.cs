using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ControlOfComputerClub.Model
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BookingRequest> BookingRequests { get; set; }
        public DbSet<Workplace> Workplaces { get; set; }
        public DbSet<V_ClientAmountSpent> V_ClientAmountSpent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<V_ClientAmountSpent>()
                .HasNoKey()
                .ToView("v_ClientAmountSpent");

            modelBuilder.Entity<BookingRequest>()
                .ToTable("BookingRequests", tb => tb.HasTrigger("trg_UpdateAmountSpent"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var conn = config.GetConnectionString("CyberClubDb");
                if (string.IsNullOrWhiteSpace(conn))
                    throw new InvalidOperationException("Не найден CyberClubDb в appsettings.json");

                options.UseSqlServer(conn);
            }
        }
    }
}

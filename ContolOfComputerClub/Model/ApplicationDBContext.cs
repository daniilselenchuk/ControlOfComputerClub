using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ControlOfComputerClub.Model
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BookingRequest> BookingRequests { get; set; }
        public DbSet<Workplace> Workplaces { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=FNDK_LAPTOP;Database=CyberClubDB;Trusted_Connection=True;" +
                "TrustServerCertificate=True;");
        }
    }
}

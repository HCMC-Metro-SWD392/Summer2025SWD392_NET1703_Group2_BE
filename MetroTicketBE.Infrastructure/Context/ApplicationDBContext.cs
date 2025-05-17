using MetroTicket.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Context
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; } = null!;
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogType> LogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Log>()
                .HasOne(l => l.LogType)
                .WithMany(lt => lt.Logs)
                .HasForeignKey(l => l.LogTypeId);
        }

    }
}

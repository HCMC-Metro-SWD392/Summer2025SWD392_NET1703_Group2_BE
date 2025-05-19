using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Context
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<LogType> LogTypes { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<FormRequest> FormRequests { get; set; }
        public DbSet<FormRequestType> FormRequestType { get; set; }
        public DbSet<PayOSMethod> PayOSMethods { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<SubscriptionTicket> SubscriptionTicket { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<TicketRoute> TicketRoutes { get; set; }
        public DbSet<TimeLine> TimeLine { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

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

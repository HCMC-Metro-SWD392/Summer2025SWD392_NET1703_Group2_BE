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
        public DbSet<TrainSegment> TimeLine { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Log
            modelBuilder.Entity<Log>()
                .HasOne(l => l.LogType)
                .WithMany(lt => lt.Logs)
                .HasForeignKey(l => l.LogTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Transaction
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transaction>()
                .HasMany(t => t.Tickets)
                .WithOne(ticket => ticket.Transaction)
                .HasForeignKey(ticket => ticket.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Status)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Train
            modelBuilder.Entity<Train>()
                .HasOne(train => train.Status)
                .WithMany(status => status.Trains)
                .HasForeignKey(train => train.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //TrainSegment
            modelBuilder.Entity<TrainSegment>()
                .HasOne(ts => ts.Train)
                .WithMany(t => t.TrainSegments)
                .HasForeignKey(ts => ts.TrainId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TrainSegment>()
                .HasOne(ts => ts.StationStart)
                .WithMany(s => s.AsStart)
                .HasForeignKey(ts => ts.StationStartId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TrainSegment>()
                .HasOne(ts => ts.StationEnd)
                .WithMany(s => s.AsEnd)
                .HasForeignKey(s => s.StationEndId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //TimeLine
            
            //Ticket Route
            modelBuilder.Entity<TicketRoute>()
                .HasOne(tr => tr.FirstStation)
                .WithMany(s => s.TicketRoutes)
                .HasForeignKey(tr => tr.FirstStationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TicketRoute>()
                .HasOne(tr => tr.LastStation)
                .WithMany(s => s.TicketRoutes)
                .HasForeignKey(tr => tr.LastStationId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.SubscriptionTicket)
                .WithMany(st => st.Tickets)
                .HasForeignKey(t => t.SubscriptionTicket)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

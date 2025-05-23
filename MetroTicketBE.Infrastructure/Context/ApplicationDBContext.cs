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
        public DbSet<Promotion> Discounts { get; set; }
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
        public DbSet<TimeLine> TimeLines { get; set; }
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
            modelBuilder.Entity<TimeLine>()
                .HasOne(tl => tl.TrainSegment)
                .WithMany(ts => ts.TimeLines)
                .HasForeignKey(tl => tl.TrainSegmentId)
                .OnDelete(DeleteBehavior.Restrict);
            //Ticket Route
            modelBuilder.Entity<TicketRoute>()
                .HasOne(tr => tr.FirstStation)
                .WithMany(s => s.TicketRoutesAsFirstStation)
                .HasForeignKey(tr => tr.FirstStationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TicketRoute>()
                .HasOne(tr => tr.LastStation)
                .WithMany(s => s.TicketRoutesAsLastStation)
                .HasForeignKey(tr => tr.LastStationId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Ticket
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.SubscriptionTicket)
                .WithMany(st => st.Tickets)
                .HasForeignKey(t => t.SubscriptionTicketId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Route)
                .WithMany(tr => tr.Tickets)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Transaction)
                .WithMany(tr => tr.Tickets)
                .HasForeignKey(t => t.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //SubscriptionTicket
            modelBuilder.Entity<SubscriptionTicket>()
                .HasOne(st => st.TicketType)
                .WithMany(tt => tt.SubscriptionTickets)
                .HasForeignKey(st => st.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Process
            modelBuilder.Entity<Process>()
                .HasOne(p => p.StationCheckIn)
                .WithMany(s => s.CheckInProcesses)
                .HasForeignKey(p => p.StationCheckInId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Process>()
                .HasOne(p => p.StationCheckOut)
                .WithMany(s => s.CheckOutProcesses)
                .HasForeignKey(p => p.StationCheckOutId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Process>()
                .HasOne(p => p.Status)
                .WithMany(s => s.Processes)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(deleteBehavior: DeleteBehavior.Restrict);
            
            //PayOSMethod
            modelBuilder.Entity<PayOSMethod>()
                .HasOne(p => p.Status)
                .WithMany(s => s.PayOSMethods)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PayOSMethod>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.PayOSMethods)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Log
            // modelBuilder.Entity<Log>()
            //     .HasOne(l => l.User)
            //     .WithMany(u => u.Logs)
            //     .HasForeignKey(l => l.CreatedById)
            //     .OnDelete(DeleteBehavior.Restrict);
            // modelBuilder.Entity<Log>()
            //     .HasOne(l => l.User)
            //     .WithMany(u => u.Logs)
            //     .HasForeignKey(l => l.UpdatedById)
            //     .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Log>()
                .HasOne(l => l.LogType)
                .WithMany(lt => lt.Logs)
                .HasForeignKey(l => l.LogTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //FormRequest
            modelBuilder.Entity<FormRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.FormRequestsAsSenders)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FormRequest>()
                .HasOne(fr => fr.FormRequestType)
                .WithMany(frt => frt.FormRequests)
                .HasForeignKey(fr => fr.FormRequestTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FormRequest>()
                .HasOne(fr => fr.Reviewer)
                .WithMany(r => r.FormRequestsAsReviewers)
                .HasForeignKey(fr => fr.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FormRequest>()
                .HasOne(fr => fr.Status)
                .WithMany(s => s.FormRequests)
                .HasForeignKey(fr => fr.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Customer
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CustomerType)
                .WithMany(ct => ct.Customers)
                .HasForeignKey(c => c.CustomerTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Membership)
                .WithMany(m => m.Customers)
                .HasForeignKey(c => c.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //Staff
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.StaffSchedule)
                .WithMany(ss => ss.Staffs)
                .HasForeignKey(s => s.StaffScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
            
            //StaffSchedule
            modelBuilder.Entity<StaffSchedule>()
                .HasOne(ss => ss.Status)
                .WithMany(s => s.StaffSchedules)
                .HasForeignKey(ss => ss.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

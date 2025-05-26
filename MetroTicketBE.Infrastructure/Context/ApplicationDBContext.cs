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
        public DbSet<Promotion> Discounts { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<FormRequest> FormRequests { get; set; }
        public DbSet<PayOSMethod> PayOSMethods { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<SubscriptionTicket> SubscriptionTicket { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<TicketRoute> TicketRoutes { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<MetroLine> MetroLines { get; set; }
        public DbSet<TrainSchedule> StrainSchedules { get; set; }
        public DbSet<MetroLineStation> MetroLineStations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                .HasOne(t => t.Promotion)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PromotionId)
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
                .HasOne(t => t.TicketRoute)
                .WithMany(tr => tr.Tickets)
                .HasForeignKey(t => t.TicketRouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Transaction)
                .WithMany(tr => tr.Tickets)
                .HasForeignKey(t => t.TransactionId)
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

            //PayOSMethod
            modelBuilder.Entity<PayOSMethod>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.PayOSMethods)
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            //FormRequest
            modelBuilder.Entity<FormRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.FormRequestsAsSenders)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FormRequest>()
                .HasOne(fr => fr.Reviewer)
                .WithMany(r => r.FormRequestsAsReviewers)
                .HasForeignKey(fr => fr.ReviewerId)
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

            //MetroLine
            modelBuilder.Entity<MetroLine>()
                .HasOne(m => m.StartStation)
                .WithMany(s => s.StartStations)
                .HasForeignKey(m => m.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MetroLine>()
                .HasOne(m => m.EndStation)
                .WithMany(s => s.EndStations)
                .HasForeignKey(m => m.EndStationId)
                .HasForeignKey(m => m.EndStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MetroLine>()
                .HasOne(m => m.FareRule)
                .WithMany(f => f.MetroLines)
                .HasForeignKey(m => m.FareRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            //MetroLineStation
            modelBuilder.Entity<MetroLineStation>()
                .HasOne(ms => ms.Station)
                .WithMany(s => s.MetroLineStations)
                .HasForeignKey(ms => ms.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MetroLineStation>()
                .HasOne(ms => ms.MetroLine)
                .WithMany(ml => ml.MetroLineStations)
                .HasForeignKey(ms => ms.MetroLineId)
                .OnDelete(DeleteBehavior.Restrict);

            //TrainSchedules
            modelBuilder.Entity<TrainSchedule>()
                .HasOne(ts => ts.StartStation)
                .WithMany(s => s.StrainSchedules)
                .HasForeignKey(ts => ts.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrainSchedule>()
    .HasMany(ts => ts.Trains)
    .WithMany(t => t.TrainSchedules)
    .UsingEntity<Dictionary<string, object>>(
        "TrainScheduleWithTrain", // Tên bảng trung gian
        j => j
            .HasOne<Train>()
            .WithMany()
            .HasForeignKey("TrainId")
            .OnDelete(DeleteBehavior.Restrict),
        j => j
            .HasOne<TrainSchedule>()
            .WithMany()
            .HasForeignKey("TrainScheduleId")
            .OnDelete(DeleteBehavior.Restrict),
        j =>
        {
            j.HasKey("TrainId", "TrainScheduleId");
            j.ToTable("TrainScheduleWithTrain"); // Tùy chọn tên bảng
        });


            //FormRequest
            modelBuilder.Entity<FormRequest>()
                .HasMany(f => f.FormAttachments)
                .WithOne(fa => fa.FormRequest)
                .HasForeignKey(fa => fa.FormRequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

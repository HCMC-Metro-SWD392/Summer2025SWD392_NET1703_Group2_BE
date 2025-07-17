using MetroTicket.Domain.Entities;
using MetroTicketBE.Domain.Entities;
using MetroTicketBE.Infrastructure.Seeding;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MetroTicketBE.Infrastructure.Context
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<FormRequest> FormRequests { get; set; }
        public DbSet<FormAttachment> FormAttachments { get; set; }
        public DbSet<PayOSMethod> PayOSMethods { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<SubscriptionTicket> SubscriptionTickets { get; set; }
        public DbSet<SubscriptionTicketType> SubscriptionTicketTypes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<MetroLine> MetroLines { get; set; }
        public DbSet<TrainSchedule> TrainSchedules { get; set; }
        public DbSet<MetroLineStation> MetroLineStations { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }
        public DbSet<StaffShift> StaffShifts { get; set; }
        public DbSet<FareRule> FareRules { get; set; }
        public DbSet<TicketRoute> TicketRoutes { get; set; }
        public DbSet<TicketProcess> TicketProcesses { get; set; }
        public DbSet<News> News { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Seeding
            //ApplicationDBContextSeed.SeedEmailTemplate(modelBuilder);

            //News
            modelBuilder.Entity<News>()
                .HasOne(n => n.Staff)
                .WithMany(u => u.News)
                .HasForeignKey(n => n.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            //TicketProcesses
            modelBuilder.Entity<TicketProcess>()
                .HasOne(tp => tp.Ticket)
                .WithMany(t => t.TicketProcesses)
                .HasForeignKey(tp => tp.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketProcess>()
                .HasOne(tp => tp.Station)
                .WithMany(s => s.TicketProcesses)
                .HasForeignKey(tp => tp.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            //PaymentTransaction

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(t => t.Promotion)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(t => t.Customer)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(ts => ts.Ticket)
                .WithMany(t => t.Transaction)
                .HasForeignKey(t => t.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(t => t.PaymentMethod)
                .WithMany(p => p.Transactions)
                .HasForeignKey(p => p.PaymentMethodId)
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

            //modelBuilder.Entity<Ticket>()
            //    .HasMany(t => t.Transaction)
            //    .WithOne(tr => tr.Ticket_)
            //    .HasForeignKey(t => t.TicketId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //TicketRoute
            modelBuilder.Entity<TicketRoute>()
                .HasOne(tr => tr.StartStation)
                .WithMany(s => s.TicketRoutesAsFirstStation)
                .HasForeignKey(tr => tr.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TicketRoute>()
                .HasOne(tr => tr.EndStation)
                .WithMany(s => s.TicketRoutesAsLastStation)
                .HasForeignKey(tr => tr.EndStationId)
                .OnDelete(DeleteBehavior.Restrict);

            //SubscriptionTicket
            modelBuilder.Entity<SubscriptionTicket>()
                .HasOne(st => st.StartStation)
                .WithMany(s => s.SubscriptionTicketsAsStartStation)
                .HasForeignKey(st => st.StartStationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SubscriptionTicket>()
                .HasOne(st => st.EndStation)
                .WithMany(s => s.SubscriptionTicketsAsEndStation)
                .HasForeignKey(st => st.EndStationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SubscriptionTicket>()
                .HasOne(st => st.TicketType)
                .WithMany(tt => tt.SubscriptionTickets)
                .HasForeignKey(st => st.TicketTypeId)
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
                .HasOne(c => c.Membership)
                .WithMany(m => m.Customers)
                .HasForeignKey(c => c.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.DateOfBirth)
                .HasColumnType("date");

            //StaffSchedule
            modelBuilder.Entity<StaffSchedule>()
                .HasOne(ss => ss.Shift)
                .WithMany(s => s.StaffSchedules)
                .HasForeignKey(ss => ss.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StaffSchedule>()
                .HasOne(ss => ss.Staff)
                .WithMany(s => s.StaffSchedules)
                .HasForeignKey(ss => ss.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StaffSchedule>()
                .HasOne(ss => ss.WorkingStation)
                .WithMany(s => s.StaffSchedules)
                .HasForeignKey(ss => ss.WorkingStationId)
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
                .HasOne(ts => ts.Station)
                .WithMany(s => s.StrainSchedules)
                .HasForeignKey(ts => ts.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrainSchedule>()
                .HasOne(ts => ts.MetroLine)
                .WithMany(ml => ml.TrainSchedules)
                .HasForeignKey(ts => ts.MetroLineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrainSchedule>()
                .HasMany(ts => ts.Trains)
                .WithMany(t => t.TrainSchedules)
                .UsingEntity<Dictionary<string, object>>(
                    "TrainScheduleWithTrain",
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
                            j.ToTable("TrainScheduleWithTrain");
                        });


            //FormAttachment
            modelBuilder.Entity<FormAttachment>()
                .HasOne(fa => fa.FormRequest)
                .WithMany(fr => fr.FormAttachments)
                .HasForeignKey(fa => fa.FormRequestId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

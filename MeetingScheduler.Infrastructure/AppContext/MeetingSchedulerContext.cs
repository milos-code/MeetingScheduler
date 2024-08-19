using MeetingScheduler.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Infrastructure.AppContext
{
    public class MeetingSchedulerContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public MeetingSchedulerContext()
        {
        }

        public MeetingSchedulerContext(DbContextOptions<MeetingSchedulerContext> options) 
            : base(options) 
        {
        }

        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingNotes> MeetingNotes { get; set; }
        public DbSet<UserMeetingStatus> UserMeetingStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(e => e.PeopleManager)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserMeetingStatus>()
                .HasKey(k => new { k.MeetingId , k.UserId});

            modelBuilder.Entity<Meeting>()
                .HasMany(u => u.Users)
                .WithMany(m => m.Meetings)
                .UsingEntity<UserMeetingStatus>();
        }
    }
}

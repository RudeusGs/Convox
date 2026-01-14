using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using server.Domain.Entities;

namespace server.Infrastructure.Persistence
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<BreakoutRoom> BreakoutRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizResponse> QuizResponses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<UserRoomControl> UserRoomControls { get; set; }
        public DbSet<ChatMessageBreakoutRoom> ChatMessageBreakoutRooms { get; set; }
        public DbSet<ChatP2P> ChatP2Ps { get; set; }

    }
}

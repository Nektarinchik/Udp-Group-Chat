using Microsoft.EntityFrameworkCore;
using UdpChatWithMessageHistory.Entities;

namespace UdpChatWithMessageHistory.EF
{
    internal class MessagesDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public MessagesDbContext()
        {

        }
        // TODO: make connection string
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-G9GG5EN;Database=RDBMSDb;TrustServerCertificate=True;User ID=nikita;Password=nikita;MultipleActiveResultSets=true");
        }

    }
}

using Fashion.Api.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Api.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
        public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Drop> Drops => Set<Drop>();
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique product slug
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            // Store OrderStatus as string instead of int
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            //Ai-Chat or Message Service
            modelBuilder.Entity<Message>()
    .HasOne(m => m.Conversation)
    .WithMany(c => c.Messages)
    .HasForeignKey(m => m.ConversationId)
    .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
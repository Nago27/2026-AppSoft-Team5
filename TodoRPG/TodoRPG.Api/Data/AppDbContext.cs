using Microsoft.EntityFrameworkCore;
using TodoRPG.Api.Models; // Models 폴더의 TodoItem을 가져오기 위함

namespace TodoRPG.Api.Data
{
    // DbContext를 상속받아 DB 연결 다리 역할을 합니다.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 이 한 줄이 가장 중요합니다! 
        // TodoItem 클래스를 데이터베이스의 'TodoItems'라는 테이블로 만들어 줍니다.
        public DbSet<TodoItem> TodoItems { get; set; }

        public DbSet<User> Users { get; set; } // ID, 닉네임, PW 담은 Users 테이블 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>()
                .HasOne(todo => todo.User)
                .WithMany()
                .HasForeignKey(todo => todo.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TodoItem>()
                .HasIndex(todo => new
                {
                    todo.UserId,
                    todo.IsCompleted,
                    todo.CreatedAt
                });
        }
    }
}
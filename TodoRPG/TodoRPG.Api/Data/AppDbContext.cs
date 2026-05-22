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
        public DbSet<Character> Characters { get; set; }
        public DbSet<Dungeon> Dungeons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TodoItem - User 관계 및 인덱스
            modelBuilder.Entity<TodoItem>()
                .HasOne(todo => todo.User)
                .WithMany()
                .HasForeignKey(todo => todo.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TodoItem>()
                .HasIndex(todo => new { todo.UserId, todo.IsCompleted, todo.CreatedAt });

            // User - Character 1:1 관계 설정
            modelBuilder.Entity<Character>()
                .HasOne(c => c.User)
                .WithOne(u => u.Character)
                .HasForeignKey<Character>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 유저 삭제시 캐릭터도 자동 삭제

            // Character - Dungeon 관계 설정
            modelBuilder.Entity<Character>()
                .HasOne(c => c.CurrentDungeon)
                .WithMany()
                .HasForeignKey(c => c.CurrentDungeonIndex)
                .OnDelete(DeleteBehavior.Restrict);

            // 던전 초기 데이터 (Seed Data) 설정
            modelBuilder.Entity<Dungeon>().HasData(
                new Dungeon { Index = 1, Name = "초보자의 숲", RequiredCondition = 1 },
                new Dungeon { Index = 2, Name = "고블린 동굴", RequiredCondition = 5 },
                new Dungeon { Index = 3, Name = "오크 요새", RequiredCondition = 10 },
                new Dungeon { Index = 4, Name = "드래곤의 둥지", RequiredCondition = 20 }
            );
        }
    }
}
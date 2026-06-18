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
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<Inventory> Inventories { get; set; }

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

            // 1. Inventory - User 외래키 연동 및 연쇄 삭제 규칙
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade); // 유저 회원 탈퇴 시 해당 가방 데이터도 완전 자동 삭제

            // 2. Inventory - ShopItem 외래키 연동 및 연쇄 삭제 규칙
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.ShopItem)
                .WithMany()
                .HasForeignKey(i => i.ShopItemId)
                .OnDelete(DeleteBehavior.Cascade); // 상점에서 상품 삭제 시 유저 인벤토리 내 해당 아이템 일괄 제거

            // 💡 내 ShopItem.cs 변수 구조 규격에 맞게 100% 검산 정정한 데이터셋
            // 💡 스텟 성장 로직 재화 수급량 분석 기반 최적화 가격 반영 데이터셋
            modelBuilder.Entity<ShopItem>().HasData(
                // ==========================================
                // 1. 근력 계열 아이템
                // ==========================================
                new ShopItem { Id = 1, Name = "k2소총", ItemType = "NoneEquip", Description = "가방에 군용 소총을 소지하는 것만으로 생존을 위한 강력한 완력이 솟아납니다. (근력 +10)", Cost = 60, PlusStat = "STR", PlusStatValue = 10, Stock = 999, Rarity = "Rare", DropWeight = 50 },
                new ShopItem { Id = 2, Name = "스미스머신", ItemType = "NoneEquip", Description = "쉘터 한구석에 거대한 운동 기구를 들여놓았습니다. 보기만 해도 근육이 성장합니다. (근력 +20)", Cost = 150, PlusStat = "STR", PlusStatValue = 20, Stock = 999, Rarity = "Epic", DropWeight = 15 },
                new ShopItem { Id = 3, Name = "마크42", ItemType = "Equip", Description = "[장착 장비] 최첨단 기계 수트를 몸에 걸쳐 인간의 한계를 초월한 괴력을 발휘합니다. (근력 +30)", Cost = 300, PlusStat = "STR", PlusStatValue = 30, Stock = 999, Rarity = "Legendary", DropWeight = 5 },

                // ==========================================
                // 2. 체력 계열 아이템
                // ==========================================
                new ShopItem { Id = 4, Name = "황색 방호복", ItemType = "Equip", Description = "[장착 장비] 방사능과 외부 독소로부터 신체를 보호하여 생존력을 높여주는 두꺼운 의류입니다. (체력 +10)", Cost = 60, PlusStat = "STA", PlusStatValue = 10, Stock = 999, Rarity = "Rare", DropWeight = 50 },
                new ShopItem { Id = 5, Name = "가고일돌갑옷", ItemType = "Equip", Description = "[장착 장비] 가고일의 단단한 피부처럼 물리적 타격을 완벽히 흡수하는 석조 갑옷입니다. (체력 +20)", Cost = 150, PlusStat = "STA", PlusStatValue = 20, Stock = 999, Rarity = "Epic", DropWeight = 15 },
                new ShopItem { Id = 6, Name = "런닝머신", ItemType = "NoneEquip", Description = "언제든 달릴 수 있는 유산소 기구입니다. 가방에 두는 것(?)만으로 심폐지구력이 강화됩니다. (체력 +30)", Cost = 300, PlusStat = "STA", PlusStatValue = 30, Stock = 999, Rarity = "Legendary", DropWeight = 5 },

                // ==========================================
                // 3. 지능 계열 아이템
                // ==========================================
                new ShopItem { Id = 7, Name = "지도", ItemType = "NoneEquip", Description = "인근 황무지의 지형 구조가 상세히 기록된 지도입니다. 탐색 시 판단력이 상승합니다. (지능 +10)", Cost = 60, PlusStat = "INT", PlusStatValue = 10, Stock = 999, Rarity = "Rare", DropWeight = 50 },
                new ShopItem { Id = 8, Name = "아이폰 시리", ItemType = "Equip", Description = "[장착 장비] 손목이나 장비에 연동하여 실시간으로 인공지능의 전술 연산 서포트를 받습니다. (지능 +20)", Cost = 150, PlusStat = "INT", PlusStatValue = 20, Stock = 999, Rarity = "Epic", DropWeight = 15 },
                new ShopItem { Id = 9, Name = "교과서", ItemType = "NoneEquip", Description = "무너진 문명의 지식이 담긴 두꺼운 학술서입니다. 읽을 때마다 연산 능력이 깊어집니다. (지능 +30)", Cost = 300, PlusStat = "INT", PlusStatValue = 30, Stock = 999, Rarity = "Legendary", DropWeight = 5 },

                // ==========================================
                // 4. 행운 계열 아이템
                // ==========================================
                new ShopItem { Id = 10, Name = "복권", ItemType = "NoneEquip", Description = "종말 전 발행된 유효기간 지난 복권입니다. 왠지 모르게 좋은 일이 생길 것 같은 예감을 줍니다. (행운 +10)", Cost = 60, PlusStat = "LUK", PlusStatValue = 10, Stock = 999, Rarity = "Rare", DropWeight = 50 },
                new ShopItem { Id = 11, Name = "네잎클로버", ItemType = "NoneEquip", Description = "오염된 수풀 사이에서 기적적으로 발견한 돌연변이 식물입니다. 기묘한 운이 따릅니다. (행운 +20)", Cost = 150, PlusStat = "LUK", PlusStatValue = 20, Stock = 999, Rarity = "Epic", DropWeight = 15 },
                new ShopItem { Id = 12, Name = "행운의편지", ItemType = "Equip", Description = "[장착 장비] 이 편지는 영국에서부터 시작되었습니다... 부적처럼 몸에 품으면 불운을 막아줍니다. (행운 +30)", Cost = 300, PlusStat = "LUK", PlusStatValue = 30, Stock = 999, Rarity = "Legendary", DropWeight = 5 },

                // ==========================================
                // 5. 소모품 및 티켓 계열 아이템
                // ==========================================
                new ShopItem { Id = 13, Name = "던전 입장권", ItemType = "Consume", Description = "위험지대로 분류된 황무지 던전에 안전하게 진입할 수 있도록 인가된 일회성 패스권", Cost = 20, PlusStat = "DUN GEON", PlusStatValue = 1, Stock = 999, Rarity = "Common", DropWeight = 100 },
                new ShopItem { Id = 14, Name = "프로틴쉐이크", ItemType = "Consume", Description = "[소모품] 마시는 즉시 단백질을 급속 충전하여 일시적으로 파괴적인 힘을 냅니다. (근력 +10)", Cost = 15, PlusStat = "STR", PlusStatValue = 10, Stock = 999, Rarity = "Common", DropWeight = 120 },
                new ShopItem { Id = 15, Name = "슈퍼솔져혈청", ItemType = "Consume", Description = "[소모품] 심장에 주사하면 일시적으로 신체 세포가 복구되며 대사 연산 기능이 극대화됩니다. (체력 +10)", Cost = 15, PlusStat = "STA", PlusStatValue = 10, Stock = 999, Rarity = "Common", DropWeight = 120 },
                new ShopItem { Id = 16, Name = "무안단물", ItemType = "Consume", Description = "[소모품] 신비한 물을 한 모금 마시자 막혔던 뇌의 신경 회로 연산이 뚫리며 총명해집니다. (지능 +10)", Cost = 15, PlusStat = "INT", PlusStatValue = 10, Stock = 999, Rarity = "Common", DropWeight = 120 },
                new ShopItem { Id = 17, Name = "웃음", ItemType = "Consume", Description = "[소모품] 긍정적인 마음으로 한 번 크게 웃으면 쉘터 안에 긍정적인 파동과 운이 찾아옵니다. (행운 +10)", Cost = 10, PlusStat = "LUK", PlusStatValue = 10, Stock = 999, Rarity = "Common", DropWeight = 150 }
            );
        }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TodoRPG.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDungeonTicketsToCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DungeonTickets",
                table: "Characters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "ShopItems",
                columns: new[] { "Id", "Cost", "Description", "ItemType", "Name", "PlusStat", "PlusStatValue", "Stock" },
                values: new object[,]
                {
                    { 1, 60, "가방에 군용 소총을 소지하는 것만으로 생존을 위한 강력한 완력이 솟아납니다. (근력 +10)", "NoneEquip", "k2소총", "STR", 10, 999 },
                    { 2, 150, "쉘터 한구석에 거대한 운동 기구를 들여놓았습니다. 보기만 해도 근육이 성장합니다. (근력 +20)", "NoneEquip", "스미스머신", "STR", 20, 999 },
                    { 3, 300, "[장착 장비] 최첨단 기계 수트를 몸에 걸쳐 인간의 한계를 초월한 괴력을 발휘합니다. (근력 +30)", "Equip", "마크42", "STR", 30, 999 },
                    { 4, 60, "[장착 장비] 방사능과 외부 독소로부터 신체를 보호하여 생존력을 높여주는 두꺼운 의류입니다. (체력 +10)", "Equip", "황색 방호복", "STA", 10, 999 },
                    { 5, 150, "[장착 장비] 가고일의 단단한 피부처럼 물리적 타격을 완벽히 흡수하는 석조 갑옷입니다. (체력 +20)", "Equip", "가고일돌갑옷", "STA", 20, 999 },
                    { 6, 300, "언제든 달릴 수 있는 유산소 기구입니다. 가방에 두는 것(?)만으로 심폐지구력이 강화됩니다. (체력 +30)", "NoneEquip", "런닝머신", "STA", 30, 999 },
                    { 7, 60, "인근 황무지의 지형 구조가 상세히 기록된 지도입니다. 탐색 시 판단력이 상승합니다. (지능 +10)", "NoneEquip", "지도", "INT", 10, 999 },
                    { 8, 150, "[장착 장비] 손목이나 장비에 연동하여 실시간으로 인공지능의 전술 연산 서포트를 받습니다. (지능 +20)", "Equip", "아이폰 시리", "INT", 20, 999 },
                    { 9, 300, "무너진 문명의 지식이 담긴 두꺼운 학술서입니다. 읽을 때마다 연산 능력이 깊어집니다. (지능 +30)", "NoneEquip", "교과서", "INT", 30, 999 },
                    { 10, 60, "종말 전 발행된 유효기간 지난 복권입니다. 왠지 모르게 좋은 일이 생길 것 같은 예감을 줍니다. (행운 +10)", "NoneEquip", "복권", "LUK", 10, 999 },
                    { 11, 150, "오염된 수풀 사이에서 기적적으로 발견한 돌연변이 식물입니다. 기묘한 운이 따릅니다. (행운 +20)", "NoneEquip", "네잎클로버", "LUK", 20, 999 },
                    { 12, 300, "[장착 장비] 이 편지는 영국에서부터 시작되었습니다... 부적처럼 몸에 품으면 불운을 막아줍니다. (행운 +30)", "Equip", "행운의편지", "LUK", 30, 999 },
                    { 13, 20, "위험지대로 분류된 황무지 던전에 안전하게 진입할 수 있도록 인가된 일회성 패스권", "Consume", "던전 입장권", "DUN GEON", 1, 999 },
                    { 14, 15, "[소모품] 마시는 즉시 단백질을 급속 충전하여 일시적으로 파괴적인 힘을 냅니다. (근력 +10)", "Consume", "프로틴쉐이크", "STR", 10, 999 },
                    { 15, 15, "[소모품] 심장에 주사하면 일시적으로 신체 세포가 복구되며 대사 연산 기능이 극대화됩니다. (체력 +10)", "Consume", "슈퍼솔져혈청", "STA", 10, 999 },
                    { 16, 15, "[소모품] 신비한 물을 한 모금 마시자 막혔던 뇌의 신경 회로 연산이 뚫리며 총명해집니다. (지능 +10)", "Consume", "무안단물", "INT", 10, 999 },
                    { 17, 10, "[소모품] 긍정적인 마음으로 한 번 크게 웃으면 쉘터 안에 긍정적인 파동과 운이 찾아옵니다. (행운 +10)", "Consume", "웃음", "LUK", 10, 999 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DropColumn(
                name: "DungeonTickets",
                table: "Characters");
        }
    }
}

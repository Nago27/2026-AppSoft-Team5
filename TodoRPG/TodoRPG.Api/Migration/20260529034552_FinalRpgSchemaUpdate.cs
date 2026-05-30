using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoRPG.Api.Migrations
{
    /// <inheritdoc />
    public partial class FinalRpgSchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DropWeight",
                table: "ShopItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Rarity",
                table: "ShopItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 50, "Rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 15, "Epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 5, "Legendary" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 50, "Rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 15, "Epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 5, "Legendary" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 50, "Rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 15, "Epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 5, "Legendary" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 50, "Rare" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 15, "Epic" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 5, "Legendary" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 100, "Common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 120, "Common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 120, "Common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 120, "Common" });

            migrationBuilder.UpdateData(
                table: "ShopItems",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DropWeight", "Rarity" },
                values: new object[] { 150, "Common" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropWeight",
                table: "ShopItems");

            migrationBuilder.DropColumn(
                name: "Rarity",
                table: "ShopItems");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TodoRPG.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAndAddRpgSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Dungeons",
                columns: table => new
                {
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RequiredCondition = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dungeons", x => x.Index);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "VARCHAR", maxLength: 10, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Experience = table.Column<int>(type: "INTEGER", nullable: false),
                    Coin = table.Column<int>(type: "INTEGER", nullable: false),
                    Strength = table.Column<int>(type: "INTEGER", nullable: false),
                    Intelligence = table.Column<int>(type: "INTEGER", nullable: false),
                    Fortune = table.Column<int>(type: "INTEGER", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentDungeonIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Characters_Dungeons_CurrentDungeonIndex",
                        column: x => x.CurrentDungeonIndex,
                        principalTable: "Dungeons",
                        principalColumn: "Index",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Dungeons",
                columns: new[] { "Index", "Name", "RequiredCondition" },
                values: new object[,]
                {
                    { 1, "초보자의 숲", 1 },
                    { 2, "고블린 동굴", 5 },
                    { 3, "오크 요새", 10 },
                    { 4, "드래곤의 둥지", 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CurrentDungeonIndex",
                table: "Characters",
                column: "CurrentDungeonIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Dungeons");

            migrationBuilder.AddColumn<int>(
                name: "Experience",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}

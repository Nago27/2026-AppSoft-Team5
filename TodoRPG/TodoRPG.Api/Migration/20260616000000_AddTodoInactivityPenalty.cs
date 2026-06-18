using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TodoRPG.Api.Data;

#nullable disable

namespace TodoRPG.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260616000000_AddTodoInactivityPenalty")]
    public partial class AddTodoInactivityPenalty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastTodoInactivityPenaltyAt",
                table: "Characters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RewardReductionExpiresAt",
                table: "Characters",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTodoInactivityPenaltyAt",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "RewardReductionExpiresAt",
                table: "Characters");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class RemovedInheritanceForGamePlatformAndGameGenre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfDelete",
                table: "GamesPlatformTypes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GamesPlatformTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GamesPlatformTypes");

            migrationBuilder.DropColumn(
                name: "DateOfDelete",
                table: "GamesGenres");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GamesGenres");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "GamesGenres");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfDelete",
                table: "GamesPlatformTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GamesPlatformTypes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GamesPlatformTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfDelete",
                table: "GamesGenres",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GamesGenres",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "GamesGenres",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addedfieldsforfilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfAdding",
                table: "Games",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfPublishing",
                table: "Games",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ViewCount",
                table: "Games",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfAdding",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "DateOfPublishing",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Games");
        }
    }
}

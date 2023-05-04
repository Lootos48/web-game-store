using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addedguidtomappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GoodsProduct",
                table: "GoodsProduct");

            migrationBuilder.AlterColumn<string>(
                name: "GameKey",
                table: "GoodsProduct",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "GoodsProduct",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoodsProduct",
                table: "GoodsProduct",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GoodsProduct",
                table: "GoodsProduct");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GoodsProduct");

            migrationBuilder.AlterColumn<string>(
                name: "GameKey",
                table: "GoodsProduct",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoodsProduct",
                table: "GoodsProduct",
                column: "GameKey");
        }
    }
}

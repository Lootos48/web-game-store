using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class AddedmappingIdfieldtoGames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GoodsProduct_GameId",
                table: "GoodsProduct");

            migrationBuilder.AddColumn<Guid>(
                name: "MappingId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsProduct_GameId",
                table: "GoodsProduct",
                column: "GameId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GoodsProduct_GameId",
                table: "GoodsProduct");

            migrationBuilder.DropColumn(
                name: "MappingId",
                table: "Games");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsProduct_GameId",
                table: "GoodsProduct",
                column: "GameId");
        }
    }
}

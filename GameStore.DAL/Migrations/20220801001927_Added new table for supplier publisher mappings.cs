using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addednewtableforsupplierpublishermappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsProduct_Games_GameId",
                table: "GoodsProduct");

            migrationBuilder.DropIndex(
                name: "IX_GoodsProduct_GameId",
                table: "GoodsProduct");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "GoodsProduct");

            migrationBuilder.DropColumn(
                name: "MappingId",
                table: "Games");

            migrationBuilder.CreateTable(
                name: "PublisherSupplier",
                columns: table => new
                {
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublisherSupplier", x => new { x.PublisherId, x.SupplierId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublisherSupplier");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "GoodsProduct",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsProduct_Games_GameId",
                table: "GoodsProduct",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

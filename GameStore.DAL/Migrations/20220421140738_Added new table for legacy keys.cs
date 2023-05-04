using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addednewtableforlegacykeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LegacyKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegacyKey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegacyKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegacyKeys_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegacyKeys_GameId",
                table: "LegacyKeys",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LegacyKeys_LegacyKey",
                table: "LegacyKeys",
                column: "LegacyKey",
                unique: true,
                filter: "[LegacyKey] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegacyKeys");
        }
    }
}

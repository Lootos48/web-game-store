using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addedtablesforlocalizations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Localizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CultureCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameLocalizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateOfDelete = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameLocalizations_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreLocalizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GenreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateOfDelete = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenreLocalizations_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformTypeLocalizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlatformTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocalizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateOfDelete = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformTypeLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformTypeLocalizations_Localizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "Localizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlatformTypeLocalizations_PlatformTypes_PlatformTypeId",
                        column: x => x.PlatformTypeId,
                        principalTable: "PlatformTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$RZ0am1JChLI9CoQGTz8QxeDjBdV1KBFsutWjL3hafwVRzYCwyWteq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$xb7V2/vPt8leTd4D4Jw1S.lnR92b2F1tLT3/KiIeNpqHWHLuFmn9m");

            migrationBuilder.CreateIndex(
                name: "IX_GameLocalizations_GameId",
                table: "GameLocalizations",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameLocalizations_LocalizationId",
                table: "GameLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreLocalizations_GenreId",
                table: "GenreLocalizations",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreLocalizations_LocalizationId",
                table: "GenreLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformTypeLocalizations_LocalizationId",
                table: "PlatformTypeLocalizations",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformTypeLocalizations_PlatformTypeId",
                table: "PlatformTypeLocalizations",
                column: "PlatformTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameLocalizations");

            migrationBuilder.DropTable(
                name: "GenreLocalizations");

            migrationBuilder.DropTable(
                name: "PlatformTypeLocalizations");

            migrationBuilder.DropTable(
                name: "Localizations");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$Ib53VCcOx4Eb5411bif59.ohiptJfTBthb8jmBThJcGxJxPKXCCR.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$xQEnii6oh/Eke7/kde58tOIZyv5fLeD4URbokQztEHTepuan1GmGS");
        }
    }
}

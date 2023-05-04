using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addedseedforlocalizationstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Localizations",
                columns: new[] { "Id", "CultureCode", "Name" },
                values: new object[,]
                {
                    { new Guid("b5527cff-36da-4b7c-8512-17ab15e02958"), "ru-RU", "Russian" },
                    { new Guid("2180ddff-ae88-4549-9d23-1a1dd32538d7"), "uk-UA", "Ukranian" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$WzrhLRRNM7rl9hzbKIoPj.v2A/sB2b/JqdWUVcAPZLXsZFIrO70UC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$9nxXI2kGWW6ypyql7Gl9jebncGNmszIhqnGXp/uLNzFJqCyV6U9fa");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Localizations",
                keyColumn: "Id",
                keyValue: new Guid("2180ddff-ae88-4549-9d23-1a1dd32538d7"));

            migrationBuilder.DeleteData(
                table: "Localizations",
                keyColumn: "Id",
                keyValue: new Guid("b5527cff-36da-4b7c-8512-17ab15e02958"));

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
        }
    }
}

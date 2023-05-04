using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addedhashedadminpassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$1abXa6eJIrEBP.iC7RPFOOqtP6VNale2AB.17kiCVMLSN3HZzIg2i");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "$2a$11$NStdv.kmqzgk2xpFyiILmOwdUcgF2Wd0yIYGF3jqHIZolqf910Iim");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000000"),
                column: "Password",
                value: "12345");
        }
    }
}

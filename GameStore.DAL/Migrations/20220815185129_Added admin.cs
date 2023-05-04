using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class Addedadmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfDelete", "Email", "IsDeleted", "Password", "Role", "Username" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000000"), null, "GameStoreAdmin@gmail.com", false, "Admin", "Administrator", "Admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfDelete", "Email", "IsDeleted", "Password", "Role", "Username" },
                values: new object[] { new Guid("20000000-0000-0000-0000-000000000000"), null, "GameStoreRoot@gmail.com", false, "12345", "Administrator", "Root" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000000"));
        }
    }
}

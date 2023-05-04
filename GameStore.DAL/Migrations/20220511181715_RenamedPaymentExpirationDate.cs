using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.DAL.Migrations
{
    public partial class RenamedPaymentExpirationDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentExpirationDate",
                table: "Orders",
                newName: "ExpirationDateUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpirationDateUtc",
                table: "Orders",
                newName: "PaymentExpirationDate");
        }
    }
}

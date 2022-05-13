using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class UpdateReservationAndSeatsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Reservations_Reservation_Id",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_Reservation_Id",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "Reservation_Id",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "isReserver",
                table: "Seats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reservation_Id",
                table: "Seats",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isReserver",
                table: "Seats",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Reservation_Id",
                table: "Seats",
                column: "Reservation_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Reservations_Reservation_Id",
                table: "Seats",
                column: "Reservation_Id",
                principalTable: "Reservations",
                principalColumn: "Reservation_Id");
        }
    }
}

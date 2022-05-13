using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class SetReservationIdInSeatTableToNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Reservations_Reservation_Id",
                table: "Seats");

            migrationBuilder.AlterColumn<int>(
                name: "Reservation_Id",
                table: "Seats",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Reservations_Reservation_Id",
                table: "Seats",
                column: "Reservation_Id",
                principalTable: "Reservations",
                principalColumn: "Reservation_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Reservations_Reservation_Id",
                table: "Seats");

            migrationBuilder.AlterColumn<int>(
                name: "Reservation_Id",
                table: "Seats",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Reservations_Reservation_Id",
                table: "Seats",
                column: "Reservation_Id",
                principalTable: "Reservations",
                principalColumn: "Reservation_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class AddedReservedSeatTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Seats",
                newName: "Seat_Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Projections",
                newName: "Projection_Id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Movies",
                newName: "Movie_Id");

            migrationBuilder.AlterColumn<double>(
                name: "Reservation_Price",
                table: "Reservations",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateTable(
                name: "ReservedSeats",
                columns: table => new
                {
                    Reservation_Id = table.Column<int>(type: "integer", nullable: false),
                    Seat_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedSeats", x => new { x.Reservation_Id, x.Seat_Id });
                    table.ForeignKey(
                        name: "FK_ReservedSeats_Reservations_Reservation_Id",
                        column: x => x.Reservation_Id,
                        principalTable: "Reservations",
                        principalColumn: "Reservation_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservedSeats_Seats_Seat_Id",
                        column: x => x.Seat_Id,
                        principalTable: "Seats",
                        principalColumn: "Seat_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservedSeats_Seat_Id",
                table: "ReservedSeats",
                column: "Seat_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedSeats");

            migrationBuilder.RenameColumn(
                name: "Seat_Id",
                table: "Seats",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Projection_Id",
                table: "Projections",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Movie_Id",
                table: "Movies",
                newName: "Id");

            migrationBuilder.AlterColumn<decimal>(
                name: "Reservation_Price",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}

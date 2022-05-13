using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinterWorkShop.Cinema.Data.Migrations
{
    public partial class updatedReservationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Projection_Id",
                table: "Reservations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Reservation_Price",
                table: "Reservations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Projection_Id",
                table: "Reservations",
                column: "Projection_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Projections_Projection_Id",
                table: "Reservations",
                column: "Projection_Id",
                principalTable: "Projections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Projections_Projection_Id",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_Projection_Id",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Projection_Id",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Reservation_Price",
                table: "Reservations");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP.Infrastructure.Migrations
{
    public partial class AddShowSeatIdinOrdersTableandupdatedStatusonly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowSeatId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ShowSeatId",
                table: "OrderDetails",
                column: "ShowSeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ShowSeats_ShowSeatId",
                table: "OrderDetails",
                column: "ShowSeatId",
                principalTable: "ShowSeats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ShowSeats_ShowSeatId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ShowSeatId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ShowSeatId",
                table: "OrderDetails");
        }
    }
}

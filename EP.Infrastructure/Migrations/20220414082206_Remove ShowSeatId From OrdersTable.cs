using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP.Infrastructure.Migrations
{
    public partial class RemoveShowSeatIdFromOrdersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ShowSeats",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "ShowSeats",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
                onDelete: ReferentialAction.NoAction);
        }
    }
}

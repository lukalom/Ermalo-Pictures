using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EP.Infrastructure.Migrations
{
    public partial class AddCinemaTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cinema",
                table: "CinemaHalls",
                newName: "CinemaId");

            migrationBuilder.CreateTable(
                name: "Cinema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinema", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CinemaHalls_CinemaId",
                table: "CinemaHalls",
                column: "CinemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CinemaHalls_Cinema_CinemaId",
                table: "CinemaHalls",
                column: "CinemaId",
                principalTable: "Cinema",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CinemaHalls_Cinema_CinemaId",
                table: "CinemaHalls");

            migrationBuilder.DropTable(
                name: "Cinema");

            migrationBuilder.DropIndex(
                name: "IX_CinemaHalls_CinemaId",
                table: "CinemaHalls");

            migrationBuilder.RenameColumn(
                name: "CinemaId",
                table: "CinemaHalls",
                newName: "Cinema");
        }
    }
}

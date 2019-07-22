using Microsoft.EntityFrameworkCore.Migrations;

namespace RoomTemp.Data.Migrations
{
    public partial class TempReadingTakenAtIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TempReading_TakenAt",
                table: "TempReading",
                column: "TakenAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TempReading_TakenAt",
                table: "TempReading");
        }
    }
}

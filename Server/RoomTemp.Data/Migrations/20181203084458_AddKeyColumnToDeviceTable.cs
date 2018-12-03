using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RoomTemp.Data.Migrations
{
    public partial class AddKeyColumnToDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Key",
                table: "Device",
                nullable: false,
                defaultValue: Guid.NewGuid());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "Device");
        }
    }
}

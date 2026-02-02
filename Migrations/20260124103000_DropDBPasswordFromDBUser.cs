using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITInventoryJLS.Migrations
{
    public partial class DropDBPasswordFromDBUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DBPassword",
                table: "DBUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DBPassword",
                table: "DBUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

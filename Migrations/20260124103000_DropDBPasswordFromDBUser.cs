using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITInventoryJLS.Migrations
{
    public partial class DropDBPasswordFromDBUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF COL_LENGTH('DBUsers','DBPassword') IS NOT NULL
    BEGIN
        DECLARE @var0 sysname;
        SELECT @var0 = [d].[name]
        FROM [sys].[default_constraints] [d]
        INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
        WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DBUsers]') AND [c].[name] = N'DBPassword');
        IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [DBUsers] DROP CONSTRAINT [' + @var0 + ']');
        ALTER TABLE [DBUsers] DROP COLUMN [DBPassword];
    END");
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

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITInventoryJLS.Migrations
{
    /// <inheritdoc />
    public partial class AddLockoutFieldsToDBUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "Waps");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Waps");

            migrationBuilder.AddColumn<int>(
                name: "FailedLoginCount",
                table: "DBUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "DBUsers",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginCount",
                table: "DBUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "DBUsers");

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "Waps",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MACAddress",
                table: "Waps",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    ContractId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmendmentCount = table.Column<int>(type: "int", nullable: false),
                    AutoRenewal = table.Column<bool>(type: "bit", nullable: false),
                    BudgetCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BusinessOwner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContractName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractValue = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    CounterpartyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPrivacySecurity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DocumentLocation = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GLCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InternalNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyObligations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LegalReviewer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NextRenewalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OwnersManagerApprover = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentFrequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentTerms = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedContracts = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReminderOwner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReminderSent = table.Column<bool>(type: "bit", nullable: false),
                    RenewalTermDetails = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReviewDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SLAPerformance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SupplierCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TerminationNoticeDays = table.Column<short>(type: "smallint", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ContractId);
                });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITInventoryJLS.Migrations
{
    /// <inheritdoc />
    public partial class MakeComputersDeviceIDIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    ContractUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContractName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CounterpartyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BusinessOwner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AutoRenewal = table.Column<bool>(type: "bit", nullable: false),
                    RenewalTermDetails = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ContractValue = table.Column<decimal>(type: "decimal(19,4)", nullable: false),
                    PaymentTerms = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KeyObligations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SLAPerformance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TerminationNoticeDays = table.Column<short>(type: "smallint", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DataPrivacySecurity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LegalReviewer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentLocation = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    DocVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignatureDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReviewDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NextRenewalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReminderOwner = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ReminderSent = table.Column<bool>(type: "bit", nullable: false),
                    BudgetCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GLCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentFrequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedContracts = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AmendmentCount = table.Column<int>(type: "int", nullable: false),
                    InternalNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnersManagerApprover = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ContractId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "Waps");

            migrationBuilder.DropColumn(
                name: "MACAddress",
                table: "Waps");
        }
    }
}

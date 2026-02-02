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
            // Add columns only if they don't already exist to make the migration idempotent
            migrationBuilder.Sql(@"IF COL_LENGTH('Waps','DeviceName') IS NULL
    BEGIN
        ALTER TABLE [Waps] ADD [DeviceName] nvarchar(50) NOT NULL DEFAULT N'';
    END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Waps','MACAddress') IS NULL
    BEGIN
        ALTER TABLE [Waps] ADD [MACAddress] nvarchar(50) NOT NULL DEFAULT N'';
    END");

            // Create Contracts table only if it doesn't already exist
            migrationBuilder.Sql(@"IF OBJECT_ID('Contracts') IS NULL
    BEGIN
        CREATE TABLE [Contracts] (
            [ContractId] int NOT NULL IDENTITY,
            [ContractUid] uniqueidentifier NOT NULL,
            [ContractNumber] nvarchar(50) NOT NULL,
            [ContractName] nvarchar(200) NOT NULL,
            [ContractType] nvarchar(50) NOT NULL,
            [CounterpartyName] nvarchar(200) NOT NULL,
            [BusinessOwner] nvarchar(100) NOT NULL,
            [StartDate] date NOT NULL,
            [EndDate] date NOT NULL,
            [AutoRenewal] bit NOT NULL,
            [RenewalTermDetails] nvarchar(200) NOT NULL,
            [Status] nvarchar(30) NOT NULL,
            [ContractValue] decimal(19,4) NOT NULL,
            [PaymentTerms] nvarchar(50) NOT NULL,
            [KeyObligations] nvarchar(max) NOT NULL,
            [SLAPerformance] nvarchar(max) NOT NULL,
            [TerminationNoticeDays] smallint NOT NULL,
            [RiskLevel] nvarchar(10) NOT NULL,
            [DataPrivacySecurity] nvarchar(200) NOT NULL,
            [LegalReviewer] nvarchar(100) NOT NULL,
            [DocumentLocation] nvarchar(400) NOT NULL,
            [DocVersion] nvarchar(20) NOT NULL,
            [LastUpdatedDate] datetime2 NOT NULL,
            [SignatureDate] date NOT NULL,
            [ReviewDate] date NOT NULL,
            [NextRenewalDate] date NOT NULL,
            [ReminderOwner] nvarchar(100) NOT NULL,
            [ReminderSent] bit NOT NULL,
            [BudgetCenter] nvarchar(50) NOT NULL,
            [GLCode] nvarchar(50) NOT NULL,
            [PaymentFrequency] nvarchar(20) NOT NULL,
            [PONumber] nvarchar(50) NOT NULL,
            [SupplierCategory] nvarchar(50) NOT NULL,
            [RelatedContracts] nvarchar(200) NOT NULL,
            [AmendmentCount] int NOT NULL,
            [InternalNotes] nvarchar(max) NOT NULL,
            [OwnersManagerApprover] nvarchar(100) NOT NULL,
            [CreatedAtUtc] datetime2 NOT NULL,
            [UpdatedAtUtc] datetime2 NOT NULL,
            CONSTRAINT [PK_Contracts] PRIMARY KEY ([ContractId])
        );
    END");
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

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.Ranch
{
    /// <inheritdoc />
    public partial class AddRanchSchemawithRationsGrowthTreatmentsandPreventiveTreatments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ranch");

            migrationBuilder.CreateTable(
                name: "GrowthTreatments",
                schema: "ranch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    DollarsPerHead = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthTreatments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreventiveTreatments",
                schema: "ranch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    DollarsPerHead = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreventiveTreatments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rations",
                schema: "ranch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    DollarsPerPound = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrowthTreatments_Name_TenantId",
                schema: "ranch",
                table: "GrowthTreatments",
                columns: new[] { "Name", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreventiveTreatments_Name_TenantId",
                schema: "ranch",
                table: "PreventiveTreatments",
                columns: new[] { "Name", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rations_Name_TenantId",
                schema: "ranch",
                table: "Rations",
                columns: new[] { "Name", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrowthTreatments",
                schema: "ranch");

            migrationBuilder.DropTable(
                name: "PreventiveTreatments",
                schema: "ranch");

            migrationBuilder.DropTable(
                name: "Rations",
                schema: "ranch");
        }
    }
}

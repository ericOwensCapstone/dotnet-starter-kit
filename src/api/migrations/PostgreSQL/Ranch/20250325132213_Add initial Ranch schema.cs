using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.Ranch
{
    /// <inheritdoc />
    public partial class AddinitialRanchschema : Migration
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

            migrationBuilder.CreateTable(
                name: "LifecycleStages",
                schema: "ranch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    RationId = table.Column<Guid>(type: "uuid", nullable: true),
                    GrowthTreatmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreventiveTreatmentId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_LifecycleStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_GrowthTreatments_GrowthTreatmentId",
                        column: x => x.GrowthTreatmentId,
                        principalSchema: "ranch",
                        principalTable: "GrowthTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_PreventiveTreatments_PreventiveTreatmentId",
                        column: x => x.PreventiveTreatmentId,
                        principalSchema: "ranch",
                        principalTable: "PreventiveTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_Rations_RationId",
                        column: x => x.RationId,
                        principalSchema: "ranch",
                        principalTable: "Rations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrowthTreatments_Name_TenantId",
                schema: "ranch",
                table: "GrowthTreatments",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "\"Deleted\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_GrowthTreatmentId",
                schema: "ranch",
                table: "LifecycleStages",
                column: "GrowthTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_Name_TenantId",
                schema: "ranch",
                table: "LifecycleStages",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "\"Deleted\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_PreventiveTreatmentId",
                schema: "ranch",
                table: "LifecycleStages",
                column: "PreventiveTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_RationId",
                schema: "ranch",
                table: "LifecycleStages",
                column: "RationId");

            migrationBuilder.CreateIndex(
                name: "IX_PreventiveTreatments_Name_TenantId",
                schema: "ranch",
                table: "PreventiveTreatments",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "\"Deleted\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rations_Name_TenantId",
                schema: "ranch",
                table: "Rations",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "\"Deleted\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifecycleStages",
                schema: "ranch");

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

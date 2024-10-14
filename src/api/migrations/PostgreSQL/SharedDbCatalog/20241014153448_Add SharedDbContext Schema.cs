using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.SharedDbCatalog
{
    /// <inheritdoc />
    public partial class AddSharedDbContextSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sharedcatalog");

            migrationBuilder.CreateTable(
                name: "GrowthTreatments",
                schema: "sharedcatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DollarsPerHead = table.Column<decimal>(type: "numeric", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthTreatments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LifecyclePrograms",
                schema: "sharedcatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<decimal>(type: "numeric", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifecyclePrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PreventativeTreatments",
                schema: "sharedcatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DollarsPerHead = table.Column<decimal>(type: "numeric", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreventativeTreatments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rations",
                schema: "sharedcatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DollarsPerPound = table.Column<decimal>(type: "numeric", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LifecycleStages",
                schema: "sharedcatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RationId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrowthTreatmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreventativeTreatmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifecycleStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_GrowthTreatments_GrowthTreatmentId",
                        column: x => x.GrowthTreatmentId,
                        principalSchema: "sharedcatalog",
                        principalTable: "GrowthTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_PreventativeTreatments_PreventativeTreatmen~",
                        column: x => x.PreventativeTreatmentId,
                        principalSchema: "sharedcatalog",
                        principalTable: "PreventativeTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_Rations_RationId",
                        column: x => x.RationId,
                        principalSchema: "sharedcatalog",
                        principalTable: "Rations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LifecycleProgramLifecycleStage",
                schema: "sharedcatalog",
                columns: table => new
                {
                    LifecycleProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    LifecycleStageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifecycleProgramLifecycleStage", x => new { x.LifecycleProgramId, x.LifecycleStageId });
                    table.ForeignKey(
                        name: "FK_LifecycleProgramLifecycleStage_LifecyclePrograms_LifecycleP~",
                        column: x => x.LifecycleProgramId,
                        principalSchema: "sharedcatalog",
                        principalTable: "LifecyclePrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifecycleProgramLifecycleStage_LifecycleStages_LifecycleSta~",
                        column: x => x.LifecycleStageId,
                        principalSchema: "sharedcatalog",
                        principalTable: "LifecycleStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleProgramLifecycleStage_LifecycleStageId",
                schema: "sharedcatalog",
                table: "LifecycleProgramLifecycleStage",
                column: "LifecycleStageId");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_GrowthTreatmentId",
                schema: "sharedcatalog",
                table: "LifecycleStages",
                column: "GrowthTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_PreventativeTreatmentId",
                schema: "sharedcatalog",
                table: "LifecycleStages",
                column: "PreventativeTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_RationId",
                schema: "sharedcatalog",
                table: "LifecycleStages",
                column: "RationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifecycleProgramLifecycleStage",
                schema: "sharedcatalog");

            migrationBuilder.DropTable(
                name: "LifecyclePrograms",
                schema: "sharedcatalog");

            migrationBuilder.DropTable(
                name: "LifecycleStages",
                schema: "sharedcatalog");

            migrationBuilder.DropTable(
                name: "GrowthTreatments",
                schema: "sharedcatalog");

            migrationBuilder.DropTable(
                name: "PreventativeTreatments",
                schema: "sharedcatalog");

            migrationBuilder.DropTable(
                name: "Rations",
                schema: "sharedcatalog");
        }
    }
}

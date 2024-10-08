using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.LifecycleStageCatalog
{
    /// <inheritdoc />
    public partial class AddLifecycleStageCatalogSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lifecyclestagecatalog");

            migrationBuilder.CreateTable(
                name: "LifecycleStages",
                schema: "lifecyclestagecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RationId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrowthTreatmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreventativeTreatmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
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
                        principalSchema: "growthtreatmentcatalog",
                        principalTable: "GrowthTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_PreventativeTreatments_PreventativeTreatmen~",
                        column: x => x.PreventativeTreatmentId,
                        principalSchema: "preventativetreatmentcatalog",
                        principalTable: "PreventativeTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifecycleStages_Rations_RationId",
                        column: x => x.RationId,
                        principalSchema: "rationcatalog",
                        principalTable: "Rations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_GrowthTreatmentId",
                schema: "lifecyclestagecatalog",
                table: "LifecycleStages",
                column: "GrowthTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_PreventativeTreatmentId",
                schema: "lifecyclestagecatalog",
                table: "LifecycleStages",
                column: "PreventativeTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LifecycleStages_RationId",
                schema: "lifecyclestagecatalog",
                table: "LifecycleStages",
                column: "RationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifecycleStages",
                schema: "lifecyclestagecatalog");
        }
    }
}

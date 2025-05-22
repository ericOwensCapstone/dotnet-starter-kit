using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.Harvest
{
    /// <inheritdoc />
    public partial class AddHarvestSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "harvest");

            migrationBuilder.CreateTable(
                name: "HarvestContractStatuses",
                schema: "harvest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestContractStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HarvestMembers",
                schema: "harvest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HarvestContracts",
                schema: "harvest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    HarvestContractStatusId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HarvestContracts_HarvestContractStatuses_HarvestContractSta~",
                        column: x => x.HarvestContractStatusId,
                        principalSchema: "harvest",
                        principalTable: "HarvestContractStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HarvestContractHarvestMember",
                schema: "harvest",
                columns: table => new
                {
                    HarvestContractId = table.Column<Guid>(type: "uuid", nullable: false),
                    HarvestMemberId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestContractHarvestMember", x => new { x.HarvestContractId, x.HarvestMemberId });
                    table.ForeignKey(
                        name: "FK_HarvestContractHarvestMember_HarvestContracts_HarvestContra~",
                        column: x => x.HarvestContractId,
                        principalSchema: "harvest",
                        principalTable: "HarvestContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HarvestContractHarvestMember_HarvestMembers_HarvestMemberId",
                        column: x => x.HarvestMemberId,
                        principalSchema: "harvest",
                        principalTable: "HarvestMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContractHarvestMember_HarvestMemberId",
                schema: "harvest",
                table: "HarvestContractHarvestMember",
                column: "HarvestMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContracts_HarvestContractStatusId",
                schema: "harvest",
                table: "HarvestContracts",
                column: "HarvestContractStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContracts_MemberId",
                schema: "harvest",
                table: "HarvestContracts",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContracts_Name_TenantId",
                schema: "harvest",
                table: "HarvestContracts",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "\"Deleted\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContracts_TenantId",
                schema: "harvest",
                table: "HarvestContracts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContractStatuses_MemberId",
                schema: "harvest",
                table: "HarvestContractStatuses",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContractStatuses_Name_TenantId",
                schema: "harvest",
                table: "HarvestContractStatuses",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "\"Deleted\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestContractStatuses_TenantId",
                schema: "harvest",
                table: "HarvestContractStatuses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestMembers_MemberId",
                schema: "harvest",
                table: "HarvestMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestMembers_Name_TenantId",
                schema: "harvest",
                table: "HarvestMembers",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "\"Deleted\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HarvestMembers_TenantId",
                schema: "harvest",
                table: "HarvestMembers",
                column: "TenantId",
                unique: true,
                filter: "\"Deleted\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HarvestContractHarvestMember",
                schema: "harvest");

            migrationBuilder.DropTable(
                name: "HarvestContracts",
                schema: "harvest");

            migrationBuilder.DropTable(
                name: "HarvestMembers",
                schema: "harvest");

            migrationBuilder.DropTable(
                name: "HarvestContractStatuses",
                schema: "harvest");
        }
    }
}

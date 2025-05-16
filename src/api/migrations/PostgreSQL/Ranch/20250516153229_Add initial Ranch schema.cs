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
                name: "ContractStatuses",
                schema: "ranch",
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
                    table.PrimaryKey("PK_ContractStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rations",
                schema: "ranch",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(99)", maxLength: 99, nullable: false),
                    Description = table.Column<string>(type: "character varying(999)", maxLength: 999, nullable: true),
                    DollarsPerPound = table.Column<decimal>(type: "numeric", nullable: false),
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
                name: "IX_ContractStatuses_Name_TenantId",
                schema: "ranch",
                table: "ContractStatuses",
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
                name: "ContractStatuses",
                schema: "ranch");

            migrationBuilder.DropTable(
                name: "Rations",
                schema: "ranch");
        }
    }
}

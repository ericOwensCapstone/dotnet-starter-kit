using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.Identity
{
    /// <inheritdoc />
    public partial class UpdateUserInvitationAddTenantEntitySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_Email_TenantId",
                schema: "identity",
                table: "UserInvitations");

            migrationBuilder.AddColumn<Guid>(
                name: "MemberId",
                schema: "identity",
                table: "UserInvitations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetTenantId",
                schema: "identity",
                table: "UserInvitations",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_Email_TargetTenantId",
                schema: "identity",
                table: "UserInvitations",
                columns: new[] { "Email", "TargetTenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_TargetTenantId",
                schema: "identity",
                table: "UserInvitations",
                column: "TargetTenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_Email_TargetTenantId",
                schema: "identity",
                table: "UserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_TargetTenantId",
                schema: "identity",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "MemberId",
                schema: "identity",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "TargetTenantId",
                schema: "identity",
                table: "UserInvitations");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_Email_TenantId",
                schema: "identity",
                table: "UserInvitations",
                columns: new[] { "Email", "TenantId" });
        }
    }
}

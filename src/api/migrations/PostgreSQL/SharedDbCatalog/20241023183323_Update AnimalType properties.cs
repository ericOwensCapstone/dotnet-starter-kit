using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.SharedDbCatalog
{
    /// <inheritdoc />
    public partial class UpdateAnimalTypeproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DollarsPerPound",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.AddColumn<double>(
                name: "ArrivalCostPerCwtMean",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ArrivalCostPerCwtStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ArrivalHeadCountMean",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ArrivalHeadCountStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ArrivalWeightMean",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ArrivalWeightStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CarcassYieldMean",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CarcassYieldStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiseaseIncidenceMean",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiseaseIncidenceStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FcrMean",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FcrStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LowerCriticalTemp",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "QualityGradeMean",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "QualityGradeStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UpperCriticalTemp",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalCostPerCwtMean",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "ArrivalCostPerCwtStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "ArrivalHeadCountMean",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "ArrivalHeadCountStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "ArrivalWeightMean",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "ArrivalWeightStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "CarcassYieldMean",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "CarcassYieldStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "DiseaseIncidenceMean",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "DiseaseIncidenceStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "FcrMean",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "FcrStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "LowerCriticalTemp",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "QualityGradeMean",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "QualityGradeStdDev",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.DropColumn(
                name: "UpperCriticalTemp",
                schema: "sharedcatalog",
                table: "AnimalTypes");

            migrationBuilder.AddColumn<decimal>(
                name: "DollarsPerPound",
                schema: "sharedcatalog",
                table: "AnimalTypes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

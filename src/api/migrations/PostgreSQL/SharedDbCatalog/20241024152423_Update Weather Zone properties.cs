using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FSH.Starter.WebApi.Migrations.PostgreSQL.SharedDbCatalog
{
    /// <inheritdoc />
    public partial class UpdateWeatherZoneproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DollarsPerPound",
                schema: "sharedcatalog",
                table: "WeatherZones");

            migrationBuilder.AddColumn<double>(
                name: "DeviationAmplitude",
                schema: "sharedcatalog",
                table: "WeatherZones",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DeviationPeriod",
                schema: "sharedcatalog",
                table: "WeatherZones",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PeakTempDate",
                schema: "sharedcatalog",
                table: "WeatherZones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "TempRange",
                schema: "sharedcatalog",
                table: "WeatherZones",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "YearlyAverageTemp",
                schema: "sharedcatalog",
                table: "WeatherZones",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviationAmplitude",
                schema: "sharedcatalog",
                table: "WeatherZones");

            migrationBuilder.DropColumn(
                name: "DeviationPeriod",
                schema: "sharedcatalog",
                table: "WeatherZones");

            migrationBuilder.DropColumn(
                name: "PeakTempDate",
                schema: "sharedcatalog",
                table: "WeatherZones");

            migrationBuilder.DropColumn(
                name: "TempRange",
                schema: "sharedcatalog",
                table: "WeatherZones");

            migrationBuilder.DropColumn(
                name: "YearlyAverageTemp",
                schema: "sharedcatalog",
                table: "WeatherZones");

            migrationBuilder.AddColumn<decimal>(
                name: "DollarsPerPound",
                schema: "sharedcatalog",
                table: "WeatherZones",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

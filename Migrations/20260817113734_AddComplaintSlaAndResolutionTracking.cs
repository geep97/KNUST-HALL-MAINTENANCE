using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnustHallMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintSlaAndResolutionTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "Complaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueAt",
                table: "Complaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                table: "Complaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResolutionConfirmed",
                table: "Complaints",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolutionConfirmedAt",
                table: "Complaints",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Complaints",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "DueAt",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "ResolutionConfirmed",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "ResolutionConfirmedAt",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Complaints");
        }
    }
}

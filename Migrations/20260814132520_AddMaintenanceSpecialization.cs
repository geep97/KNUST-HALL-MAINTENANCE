using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnustHallMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceSpecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaintenanceSpecialization",
                table: "Users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceSpecialization",
                table: "Users");
        }
    }
}

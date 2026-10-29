using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnustHallMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Hall",
                table: "Complaints",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Hall",
                table: "Complaints",
                column: "Hall");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Complaints_Hall",
                table: "Complaints");

            migrationBuilder.AlterColumn<string>(
                name: "Hall",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}

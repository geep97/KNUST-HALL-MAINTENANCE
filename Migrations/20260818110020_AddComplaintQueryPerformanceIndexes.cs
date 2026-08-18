using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnustHallMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintQueryPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComplaintUpdates_ComplaintId",
                table: "ComplaintUpdates");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_AssignedToId",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_StudentId",
                table: "Complaints");

            migrationBuilder.AlterColumn<string>(
                name: "Hall",
                table: "Complaints",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintUpdates_ComplaintId_Timestamp",
                table: "ComplaintUpdates",
                columns: new[] { "ComplaintId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AssignedToId_DateCreated",
                table: "Complaints",
                columns: new[] { "AssignedToId", "DateCreated" });

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Hall_DateCreated",
                table: "Complaints",
                columns: new[] { "Hall", "DateCreated" });

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_StudentId_DateCreated",
                table: "Complaints",
                columns: new[] { "StudentId", "DateCreated" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComplaintUpdates_ComplaintId_Timestamp",
                table: "ComplaintUpdates");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_AssignedToId_DateCreated",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_Hall_DateCreated",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_StudentId_DateCreated",
                table: "Complaints");

            migrationBuilder.AlterColumn<string>(
                name: "Hall",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintUpdates_ComplaintId",
                table: "ComplaintUpdates",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AssignedToId",
                table: "Complaints",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_StudentId",
                table: "Complaints",
                column: "StudentId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnustHallMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAuthenticationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TelephoneNumber",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "ComplaintUpdates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Complaints",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintUpdates_ApplicationUserId",
                table: "ComplaintUpdates",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_ApplicationUserId",
                table: "Complaints",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_ApplicationUserId",
                table: "Complaints",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintUpdates_Users_ApplicationUserId",
                table: "ComplaintUpdates",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_ApplicationUserId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintUpdates_Users_ApplicationUserId",
                table: "ComplaintUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintUpdates_ApplicationUserId",
                table: "ComplaintUpdates");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_ApplicationUserId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TelephoneNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ComplaintUpdates");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Complaints");
        }
    }
}

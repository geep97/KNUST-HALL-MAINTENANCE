using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnustHallMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeComplaintStudentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_ApplicationUserId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_StudentId",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_ApplicationUserId",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Complaints");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentId",
                table: "Students",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Students_StudentId",
                table: "Complaints",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Students_StudentId",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Students_Email",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentId",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Complaints",
                type: "int",
                nullable: true);

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
                name: "FK_Complaints_Users_StudentId",
                table: "Complaints",
                column: "StudentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

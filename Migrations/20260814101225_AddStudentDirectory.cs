using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnustHallMaintenance.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentDirectory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    StudentId = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false),

                    FullName = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),

                    Email = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),

                    Programme = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),

                    Department = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true),

                    College = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true),

                    Level = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Hall = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    RoomNumber = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
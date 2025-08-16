using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stajyeryotom.Migrations
{
    /// <inheritdoc />
    public partial class Messages2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Messages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Messages");
        }
    }
}

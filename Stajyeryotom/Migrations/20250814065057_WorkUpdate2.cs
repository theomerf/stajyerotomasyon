using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stajyeryotom.Migrations
{
    /// <inheritdoc />
    public partial class WorkUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Works",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Works",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Works");
        }
    }
}

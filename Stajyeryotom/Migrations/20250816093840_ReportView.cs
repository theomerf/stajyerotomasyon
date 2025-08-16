using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stajyeryotom.Migrations
{
    /// <inheritdoc />
    public partial class ReportView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Works");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Works",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

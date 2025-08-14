using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stajyeryotom.Migrations
{
    /// <inheritdoc />
    public partial class WorkUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BroadcastType",
                table: "Works",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BroadcastType",
                table: "Works");
        }
    }
}

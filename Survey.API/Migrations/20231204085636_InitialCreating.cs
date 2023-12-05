using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "title",
                table: "Surveys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

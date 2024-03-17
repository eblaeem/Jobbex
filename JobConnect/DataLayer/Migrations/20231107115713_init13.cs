using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class init13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiredEducation",
                table: "Jobs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiredInformation",
                table: "Jobs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiredJob",
                table: "Jobs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiredLanguage",
                table: "Jobs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiredLoginToSite",
                table: "Jobs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiredPriorities",
                table: "Jobs",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiredSkills",
                table: "Jobs",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredEducation",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequiredInformation",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequiredJob",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequiredLanguage",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequiredLoginToSite",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequiredPriorities",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequiredSkills",
                table: "Jobs");
        }
    }
}

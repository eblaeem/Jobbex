using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SoftwareSkillGroupadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoftwareSkillGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareSkillGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareSkills_SoftwareSkillGroupId",
                table: "SoftwareSkills",
                column: "SoftwareSkillGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_SoftwareSkills_SoftwareSkillGroups_SoftwareSkillGroupId",
                table: "SoftwareSkills",
                column: "SoftwareSkillGroupId",
                principalTable: "SoftwareSkillGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoftwareSkills_SoftwareSkillGroups_SoftwareSkillGroupId",
                table: "SoftwareSkills");

            migrationBuilder.DropTable(
                name: "SoftwareSkillGroups");

            migrationBuilder.DropIndex(
                name: "IX_SoftwareSkills_SoftwareSkillGroupId",
                table: "SoftwareSkills");
        }
    }
}

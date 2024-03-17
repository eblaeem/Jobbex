using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SoftwareSkillSoftwareSkillGroupidnullableremoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoftwareSkills_SoftwareSkillGroups_SoftwareSkillGroupId",
                table: "SoftwareSkills");

            migrationBuilder.AlterColumn<int>(
                name: "SoftwareSkillGroupId",
                table: "SoftwareSkills",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SoftwareSkills_SoftwareSkillGroups_SoftwareSkillGroupId",
                table: "SoftwareSkills",
                column: "SoftwareSkillGroupId",
                principalTable: "SoftwareSkillGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoftwareSkills_SoftwareSkillGroups_SoftwareSkillGroupId",
                table: "SoftwareSkills");

            migrationBuilder.AlterColumn<int>(
                name: "SoftwareSkillGroupId",
                table: "SoftwareSkills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SoftwareSkills_SoftwareSkillGroups_SoftwareSkillGroupId",
                table: "SoftwareSkills",
                column: "SoftwareSkillGroupId",
                principalTable: "SoftwareSkillGroups",
                principalColumn: "Id");
        }
    }
}

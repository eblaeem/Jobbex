using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "JobBenefits");

            migrationBuilder.AddColumn<int>(
                name: "BenefitId",
                table: "JobBenefits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "JobBenefits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Benefit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benefit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobBenefits_BenefitId",
                table: "JobBenefits",
                column: "BenefitId");

            migrationBuilder.CreateIndex(
                name: "IX_JobBenefits_JobId",
                table: "JobBenefits",
                column: "JobId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_JobBenefits_Benefit_BenefitId",
            //    table: "JobBenefits",
            //    column: "BenefitId",
            //    principalTable: "Benefit",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_JobBenefits_Jobs_JobId",
            //    table: "JobBenefits",
            //    column: "JobId",
            //    principalTable: "Jobs",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobBenefits_Benefit_BenefitId",
                table: "JobBenefits");

            migrationBuilder.DropForeignKey(
                name: "FK_JobBenefits_Jobs_JobId",
                table: "JobBenefits");

            migrationBuilder.DropTable(
                name: "Benefit");

            migrationBuilder.DropIndex(
                name: "IX_JobBenefits_BenefitId",
                table: "JobBenefits");

            migrationBuilder.DropIndex(
                name: "IX_JobBenefits_JobId",
                table: "JobBenefits");

            migrationBuilder.DropColumn(
                name: "BenefitId",
                table: "JobBenefits");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "JobBenefits");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "JobBenefits",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

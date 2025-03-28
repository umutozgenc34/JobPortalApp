using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortalApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddedNavigationPropertiesToCategoryAndJobPostingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "JobPostings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CategoryId",
                table: "JobPostings",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPostings_Categories_CategoryId",
                table: "JobPostings",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPostings_Categories_CategoryId",
                table: "JobPostings");

            migrationBuilder.DropIndex(
                name: "IX_JobPostings_CategoryId",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "JobPostings");
        }
    }
}

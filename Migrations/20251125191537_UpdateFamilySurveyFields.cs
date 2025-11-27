using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhilanthroPoints.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFamilySurveyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeopleCount",
                table: "FamilySurveyResponses");

            migrationBuilder.DropColumn(
                name: "ResponseCount",
                table: "FamilySurveyResponses");

            migrationBuilder.RenameColumn(
                name: "Reaction",
                table: "FamilySurveyResponses",
                newName: "LikelihoodToRecommend");

            migrationBuilder.RenameColumn(
                name: "Likelihood",
                table: "FamilySurveyResponses",
                newName: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "ChildReaction",
                table: "FamilySurveyResponses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildReaction",
                table: "FamilySurveyResponses");

            migrationBuilder.RenameColumn(
                name: "LikelihoodToRecommend",
                table: "FamilySurveyResponses",
                newName: "Reaction");

            migrationBuilder.RenameColumn(
                name: "Comments",
                table: "FamilySurveyResponses",
                newName: "Likelihood");

            migrationBuilder.AddColumn<int>(
                name: "PeopleCount",
                table: "FamilySurveyResponses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResponseCount",
                table: "FamilySurveyResponses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}

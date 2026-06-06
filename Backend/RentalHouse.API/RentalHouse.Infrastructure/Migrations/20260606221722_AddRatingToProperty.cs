using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Properties",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Properties");
        }
    }
}

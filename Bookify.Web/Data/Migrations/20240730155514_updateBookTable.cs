using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAvaliableForRental",
                table: "Books",
                newName: "IsAvailableForRental");

            migrationBuilder.AddColumn<int>(
                name: "ImagePublicId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageThumbnailUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ImageThumbnailUrl",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "IsAvailableForRental",
                table: "Books",
                newName: "IsAvaliableForRental");
        }
    }
}

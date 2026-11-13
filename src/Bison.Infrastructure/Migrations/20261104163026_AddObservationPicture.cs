using Microsoft.EntityFrameworkCore.Migrations;
using Bison.Infrastructure.Data;

#nullable disable

namespace Bison.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObservationPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Posts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Posts");
        }
    }
}

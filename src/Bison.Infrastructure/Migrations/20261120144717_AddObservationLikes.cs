using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObservationLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObservationLikes",
                columns: table => new
                {
                    ObservationId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationLikes", x => new { x.ObservationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ObservationLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObservationLikes_Posts_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Posts",
                        principalColumn: "PostId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObservationLikes_UserId",
                table: "ObservationLikes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObservationLikes");
        }
    }
}

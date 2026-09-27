using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryNode.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedOwnerToEdge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Edges",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Edges_OwnerId",
                table: "Edges",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Users_OwnerId",
                table: "Edges",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Users_OwnerId",
                table: "Edges");

            migrationBuilder.DropIndex(
                name: "IX_Edges_OwnerId",
                table: "Edges");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Edges");
        }
    }
}

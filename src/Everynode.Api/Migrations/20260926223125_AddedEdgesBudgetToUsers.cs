using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveryNode.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedEdgesBudgetToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EdgesBudget",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdgesBudget",
                table: "Users");
        }
    }
}

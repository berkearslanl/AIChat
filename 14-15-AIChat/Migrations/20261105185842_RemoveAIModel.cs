using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _14_15_AIChat.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAIModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIModel",
                table: "Conversations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AIModel",
                table: "Conversations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

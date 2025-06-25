using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicentaBalteanu.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSufferingToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isSuffering",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isSuffering",
                table: "AspNetUsers");
        }
    }
}

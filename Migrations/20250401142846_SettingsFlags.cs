using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpLN.Migrations
{
    /// <inheritdoc />
    public partial class SettingsFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseCustomBolt12",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseLnUrlP",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseLnUrlString",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseCustomBolt12",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UseLnUrlP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UseLnUrlString",
                table: "AspNetUsers");
        }
    }
}

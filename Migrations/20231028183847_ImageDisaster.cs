using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace disasterrelief_be.Migrations
{
    public partial class ImageDisaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Disasters",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Disasters");
        }
    }
}

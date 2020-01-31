using Microsoft.EntityFrameworkCore.Migrations;

namespace AspCoreCardGameEngine.Api.Migrations
{
    public partial class Add__Game__Mode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Games");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspCoreCardGameEngine.Api.Migrations
{
    public partial class Add_Game_State__Pile_unique_Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pile_Games_GameId",
                table: "Pile");

            migrationBuilder.DropIndex(
                name: "IX_Pile_GameId",
                table: "Pile");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameId",
                table: "Pile",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Games",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pile_GameId_Type_Identifier",
                table: "Pile",
                columns: new[] { "GameId", "Type", "Identifier" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pile_Games_GameId",
                table: "Pile",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pile_Games_GameId",
                table: "Pile");

            migrationBuilder.DropIndex(
                name: "IX_Pile_GameId_Type_Identifier",
                table: "Pile");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Games");

            migrationBuilder.AlterColumn<Guid>(
                name: "GameId",
                table: "Pile",
                type: "char(36)",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateIndex(
                name: "IX_Pile_GameId",
                table: "Pile",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pile_Games_GameId",
                table: "Pile",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

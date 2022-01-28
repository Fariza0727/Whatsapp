using Microsoft.EntityFrameworkCore.Migrations;

namespace Whatsapp.Migrations
{
    public partial class z1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuAnterior",
                table: "Chatbots");

            migrationBuilder.DropColumn(
                name: "SessaoAtual",
                table: "Chatbots");

            migrationBuilder.AddColumn<int>(
                name: "NaoLidas",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NaoLidas",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "MenuAnterior",
                table: "Chatbots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SessaoAtual",
                table: "Chatbots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ONFarm.Infrastructure.Migrations
{
    public partial class RemoveEmailAgentAPCI_AddAdresse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AgentAPCI",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Patients",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patients",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AgentAPCI",
                table: "Patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
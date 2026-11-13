using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeGestão.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Pessoas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Organizadores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Organizadores");
        }
    }
}

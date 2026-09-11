using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Migrations
{
    /// <inheritdoc />
    public partial class AddMoveAndAbilityEffects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Effect",
                table: "Moves",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PP",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Effect",
                table: "Abilities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effect",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "PP",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "Effect",
                table: "Abilities");
        }
    }
}

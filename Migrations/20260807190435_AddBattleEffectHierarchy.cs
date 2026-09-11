using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleEffectHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effect",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "Effect",
                table: "Abilities");

            migrationBuilder.AddColumn<int>(
                name: "ApplyStatus",
                table: "Moves",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EffectType",
                table: "Moves",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryEffectChance",
                table: "Moves",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StageChange",
                table: "Moves",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetStat",
                table: "Moves",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EffectType",
                table: "Abilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StageChange",
                table: "Abilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusInteraction",
                table: "Abilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetStat",
                table: "Abilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Trigger",
                table: "Abilities",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyStatus",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "EffectType",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "SecondaryEffectChance",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "StageChange",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "TargetStat",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "EffectType",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "StageChange",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "StatusInteraction",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "TargetStat",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "Trigger",
                table: "Abilities");

            migrationBuilder.AddColumn<string>(
                name: "Effect",
                table: "Moves",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Effect",
                table: "Abilities",
                type: "text",
                nullable: true);
        }
    }
}

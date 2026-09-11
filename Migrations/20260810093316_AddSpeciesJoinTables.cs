using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeciesJoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abilities_PokemonSpecies_PokemonSpeciesId",
                table: "Abilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Moves_PokemonSpecies_PokemonSpeciesId",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Moves_PokemonSpeciesId",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Abilities_PokemonSpeciesId",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "PokemonSpeciesId",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "PokemonSpeciesId",
                table: "Abilities");

            migrationBuilder.CreateTable(
                name: "PokemonSpeciesAbility",
                columns: table => new
                {
                    PokemonSpeciesId = table.Column<int>(type: "integer", nullable: false),
                    AbilityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonSpeciesAbility", x => new { x.PokemonSpeciesId, x.AbilityId });
                    table.ForeignKey(
                        name: "FK_PokemonSpeciesAbility_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonSpeciesAbility_PokemonSpecies_PokemonSpeciesId",
                        column: x => x.PokemonSpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PokemonSpeciesMove",
                columns: table => new
                {
                    PokemonSpeciesId = table.Column<int>(type: "integer", nullable: false),
                    MoveId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonSpeciesMove", x => new { x.PokemonSpeciesId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_PokemonSpeciesMove_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PokemonSpeciesMove_PokemonSpecies_PokemonSpeciesId",
                        column: x => x.PokemonSpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonSpeciesAbility_AbilityId",
                table: "PokemonSpeciesAbility",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_PokemonSpeciesMove_MoveId",
                table: "PokemonSpeciesMove",
                column: "MoveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonSpeciesAbility");

            migrationBuilder.DropTable(
                name: "PokemonSpeciesMove");

            migrationBuilder.AddColumn<int>(
                name: "PokemonSpeciesId",
                table: "Moves",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PokemonSpeciesId",
                table: "Abilities",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_PokemonSpeciesId",
                table: "Moves",
                column: "PokemonSpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_PokemonSpeciesId",
                table: "Abilities",
                column: "PokemonSpeciesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Abilities_PokemonSpecies_PokemonSpeciesId",
                table: "Abilities",
                column: "PokemonSpeciesId",
                principalTable: "PokemonSpecies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Moves_PokemonSpecies_PokemonSpeciesId",
                table: "Moves",
                column: "PokemonSpeciesId",
                principalTable: "PokemonSpecies",
                principalColumn: "Id");
        }
    }
}

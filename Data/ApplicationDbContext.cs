using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.Entities;

namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PokemonSpecies> PokemonSpecies => Set<PokemonSpecies>();
        public DbSet<Move> Moves => Set<Move>();
        public DbSet<Ability> Abilities => Set<Ability>();
        public DbSet<Evolution> Evolutions => Set<Evolution>();
        public DbSet<HeldItem> HeldItems => Set<HeldItem>();
        public DbSet<OwnedPokemon> OwnedPokemon => Set<OwnedPokemon>();
        public DbSet<BattleLog> BattleLogs => Set<BattleLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Evolutions
            builder.Entity<Evolution>()
                .HasOne(e => e.FromSpecies)
                .WithMany(s => s.Evolutions)
                .HasForeignKey(e => e.FromSpeciesId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Evolution>()
                .HasOne(e => e.ToSpecies)
                .WithMany()
                .HasForeignKey(e => e.ToSpeciesId)
                .OnDelete(DeleteBehavior.Restrict);

            // OwnedPokemon -> Moves
            builder.Entity<OwnedPokemon>()
                .HasMany(p => p.Moves)
                .WithMany();

            // OwnedPokemon -> Ability
            builder.Entity<OwnedPokemon>()
                .HasOne(p => p.Ability)
                .WithMany()
                .HasForeignKey(p => p.AbilityId)
                .OnDelete(DeleteBehavior.SetNull);

            // OwnedPokemon -> HeldItem
            builder.Entity<OwnedPokemon>()
                .HasOne(p => p.HeldItem)
                .WithMany()
                .HasForeignKey(p => p.HeldItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // SPECIES -> MOVES (Explicit Many-to-Many)
            builder.Entity<PokemonSpecies>()
                .HasMany(s => s.Moves)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PokemonSpeciesMove",
                    j => j.HasOne<Move>().WithMany().HasForeignKey("MoveId"),
                    j => j.HasOne<PokemonSpecies>().WithMany().HasForeignKey("PokemonSpeciesId"),
                    j => j.HasKey("PokemonSpeciesId", "MoveId")
                );

            // SPECIES -> ABILITIES (Explicit Many-to-Many)
            builder.Entity<PokemonSpecies>()
                .HasMany(s => s.Abilities)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PokemonSpeciesAbility",
                    j => j.HasOne<Ability>().WithMany().HasForeignKey("AbilityId"),
                    j => j.HasOne<PokemonSpecies>().WithMany().HasForeignKey("PokemonSpeciesId"),
                    j => j.HasKey("PokemonSpeciesId", "AbilityId")
                );
        }
    }
}
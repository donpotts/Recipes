using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlazorRecipes.Models;
using BlazorRecipes.Shared.Models;

namespace BlazorRecipes.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Recipes> Recipes => Set<Recipes>();
    public DbSet<Tags> Tags => Set<Tags>();
    public DbSet<Ingredients> Ingredients => Set<Ingredients>();
    public DbSet<Units> Units => Set<Units>();
    public DbSet<RecipeIngredients> RecipeIngredients => Set<RecipeIngredients>();
    public DbSet<Reviews> Reviews => Set<Reviews>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Recipes>()
            .HasMany(x => x.Tags);
        modelBuilder.Entity<Recipes>()
            .HasMany(x => x.Ingredients);
        modelBuilder.Entity<Recipes>()
            .HasMany(x => x.Units);
        modelBuilder.Entity<Recipes>()
            .HasMany(x => x.Reviews);
        modelBuilder.Entity<Tags>()
            .HasMany(x => x.Recipes);
        modelBuilder.Entity<RecipeIngredients>()
            .HasOne(x => x.Recipes);
        modelBuilder.Entity<RecipeIngredients>()
            .HasOne(x => x.Ingredients);
        modelBuilder.Entity<RecipeIngredients>()
            .HasOne(x => x.Units);
        modelBuilder.Entity<Reviews>()
            .Property(e => e.Rating)
            .HasConversion<double>();
        modelBuilder.Entity<Reviews>()
            .Property(e => e.Rating)
            .HasPrecision(19, 4);
    }
}

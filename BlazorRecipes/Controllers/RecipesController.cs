using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using BlazorRecipes.Data;
using BlazorRecipes.Shared.Models;

namespace BlazorRecipes.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("Fixed")]
public class RecipesController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Recipes>> Get()
    {
        return Ok(ctx.Recipes.AsNoTracking().Include(x => x.Tags).Include(x => x.Ingredients).Include(x => x.Units).Include(x => x.Reviews));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipes>> GetAsync(long key)
    {
        var recipes = await ctx.Recipes.AsNoTracking().Include(x => x.Tags).Include(x => x.Ingredients).Include(x => x.Units).Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == key);

        if (recipes == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(recipes);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Recipes>> PostAsync(Recipes recipes)
    {
        var record = await ctx.Recipes.FindAsync(recipes.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        var tags = recipes.Tags;
        recipes.Tags = null;

        var ingredients = recipes.Ingredients;
        recipes.Ingredients = null;

        var units = recipes.Units;
        recipes.Units = null;

        var reviews = recipes.Reviews;
        recipes.Reviews = null;

        await ctx.Recipes.AddAsync(recipes);

        if (tags != null)
        {
            var newValues = await ctx.Tags.Where(x => tags.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            recipes.Tags = [..newValues];
        }

        if (ingredients != null)
        {
            var newValues = await ctx.Ingredients.Where(x => ingredients.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            recipes.Ingredients = [..newValues];
        }

        if (units != null)
        {
            var newValues = await ctx.Units.Where(x => units.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            recipes.Units = [..newValues];
        }

        if (reviews != null)
        {
            var newValues = await ctx.Reviews.Where(x => reviews.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            recipes.Reviews = [..newValues];
        }

        await ctx.SaveChangesAsync();

        return Created($"/recipes/{recipes.Id}", recipes);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipes>> PutAsync(long key, Recipes update)
    {
        var recipes = await ctx.Recipes.Include(x => x.Tags).Include(x => x.Ingredients).Include(x => x.Units).Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == key);

        if (recipes == null)
        {
            return NotFound();
        }

        ctx.Entry(recipes).CurrentValues.SetValues(update);

        if (update.Tags != null)
        {
            var updateValues = update.Tags.Select(x => x.Id);
            recipes.Tags ??= [];
            recipes.Tags.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !recipes.Tags.Select(y => y.Id).Contains(x));
            var newValues = await ctx.Tags.Where(x => addValues.Contains(x.Id)).ToListAsync();
            recipes.Tags.AddRange(newValues);
        }

        if (update.Ingredients != null)
        {
            var updateValues = update.Ingredients.Select(x => x.Id);
            recipes.Ingredients ??= [];
            recipes.Ingredients.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !recipes.Ingredients.Select(y => y.Id).Contains(x));
            var newValues = await ctx.Ingredients.Where(x => addValues.Contains(x.Id)).ToListAsync();
            recipes.Ingredients.AddRange(newValues);
        }

        if (update.Units != null)
        {
            var updateValues = update.Units.Select(x => x.Id);
            recipes.Units ??= [];
            recipes.Units.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !recipes.Units.Select(y => y.Id).Contains(x));
            var newValues = await ctx.Units.Where(x => addValues.Contains(x.Id)).ToListAsync();
            recipes.Units.AddRange(newValues);
        }

        if (update.Reviews != null)
        {
            var updateValues = update.Reviews.Select(x => x.Id);
            recipes.Reviews ??= [];
            recipes.Reviews.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !recipes.Reviews.Select(y => y.Id).Contains(x));
            var newValues = await ctx.Reviews.Where(x => addValues.Contains(x.Id)).ToListAsync();
            recipes.Reviews.AddRange(newValues);
        }

        await ctx.SaveChangesAsync();

        return Ok(recipes);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Recipes>> PatchAsync(long key, Delta<Recipes> delta)
    {
        var recipes = await ctx.Recipes.Include(x => x.Tags).Include(x => x.Ingredients).Include(x => x.Units).Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == key);

        if (recipes == null)
        {
            return NotFound();
        }

        delta.Patch(recipes);

        await ctx.SaveChangesAsync();

        return Ok(recipes);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var recipes = await ctx.Recipes.FindAsync(key);

        if (recipes != null)
        {
            ctx.Recipes.Remove(recipes);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}

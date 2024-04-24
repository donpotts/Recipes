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
public class RecipeIngredientsController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<RecipeIngredients>> Get()
    {
        return Ok(ctx.RecipeIngredients.Include(x => x.Recipes).Include(x => x.Ingredients).Include(x => x.Units));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeIngredients>> GetAsync(long key)
    {
        var recipeIngredients = await ctx.RecipeIngredients.Include(x => x.Recipes).Include(x => x.Ingredients).Include(x => x.Units).FirstOrDefaultAsync(x => x.Id == key);

        if (recipeIngredients == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(recipeIngredients);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RecipeIngredients>> PostAsync(RecipeIngredients recipeIngredients)
    {
        var record = await ctx.RecipeIngredients.FindAsync(recipeIngredients.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        await ctx.RecipeIngredients.AddAsync(recipeIngredients);

        await ctx.SaveChangesAsync();

        return Created($"/recipeingredients/{recipeIngredients.Id}", recipeIngredients);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeIngredients>> PutAsync(long key, RecipeIngredients update)
    {
        var recipeIngredients = await ctx.RecipeIngredients.FirstOrDefaultAsync(x => x.Id == key);

        if (recipeIngredients == null)
        {
            return NotFound();
        }

        ctx.Entry(recipeIngredients).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(recipeIngredients);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeIngredients>> PatchAsync(long key, Delta<RecipeIngredients> delta)
    {
        var recipeIngredients = await ctx.RecipeIngredients.FirstOrDefaultAsync(x => x.Id == key);

        if (recipeIngredients == null)
        {
            return NotFound();
        }

        delta.Patch(recipeIngredients);

        await ctx.SaveChangesAsync();

        return Ok(recipeIngredients);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var recipeIngredients = await ctx.RecipeIngredients.FindAsync(key);

        if (recipeIngredients != null)
        {
            ctx.RecipeIngredients.Remove(recipeIngredients);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}

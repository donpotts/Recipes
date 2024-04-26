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
public class IngredientsController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Ingredients>> Get()
    {
        return Ok(ctx.Ingredients.OrderBy(i => i.Name));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Ingredients>> GetAsync(long key)
    {
        var ingredients = await ctx.Ingredients.FirstOrDefaultAsync(x => x.Id == key);

        if (ingredients == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(ingredients);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Ingredients>> PostAsync(Ingredients ingredients)
    {
        var record = await ctx.Ingredients.FindAsync(ingredients.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        await ctx.Ingredients.AddAsync(ingredients);

        await ctx.SaveChangesAsync();

        return Created($"/ingredients/{ingredients.Id}", ingredients);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Ingredients>> PutAsync(long key, Ingredients update)
    {
        var ingredients = await ctx.Ingredients.FirstOrDefaultAsync(x => x.Id == key);

        if (ingredients == null)
        {
            return NotFound();
        }

        ctx.Entry(ingredients).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(ingredients);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Ingredients>> PatchAsync(long key, Delta<Ingredients> delta)
    {
        var ingredients = await ctx.Ingredients.FirstOrDefaultAsync(x => x.Id == key);

        if (ingredients == null)
        {
            return NotFound();
        }

        delta.Patch(ingredients);

        await ctx.SaveChangesAsync();

        return Ok(ingredients);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var ingredients = await ctx.Ingredients.FindAsync(key);

        if (ingredients != null)
        {
            ctx.Ingredients.Remove(ingredients);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}

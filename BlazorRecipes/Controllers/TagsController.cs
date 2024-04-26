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
public class TagsController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Tags>> Get()
    {
        return Ok(ctx.Tags.OrderBy(i => i.Name).Include(x => x.Recipes));
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Tags>> GetAsync(long key)
    {
        var tags = await ctx.Tags.Include(x => x.Recipes).FirstOrDefaultAsync(x => x.Id == key);

        if (tags == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(tags);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Tags>> PostAsync(Tags tags)
    {
        var record = await ctx.Tags.FindAsync(tags.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        var recipes = tags.Recipes;
        tags.Recipes = null;

        await ctx.Tags.AddAsync(tags);

        if (recipes != null)
        {
            var newValues = await ctx.Recipes.Where(x => recipes.Select(y => y.Id).Contains(x.Id)).ToListAsync();
            tags.Recipes = [..newValues];
        }

        await ctx.SaveChangesAsync();

        return Created($"/tags/{tags.Id}", tags);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Tags>> PutAsync(long key, Tags update)
    {
        var tags = await ctx.Tags.Include(x => x.Recipes).FirstOrDefaultAsync(x => x.Id == key);

        if (tags == null)
        {
            return NotFound();
        }

        ctx.Entry(tags).CurrentValues.SetValues(update);

        if (update.Recipes != null)
        {
            var updateValues = update.Recipes.Select(x => x.Id);
            tags.Recipes ??= [];
            tags.Recipes.RemoveAll(x => !updateValues.Contains(x.Id));
            var addValues = updateValues.Where(x => !tags.Recipes.Select(y => y.Id).Contains(x));
            var newValues = await ctx.Recipes.Where(x => addValues.Contains(x.Id)).ToListAsync();
            tags.Recipes.AddRange(newValues);
        }

        await ctx.SaveChangesAsync();

        return Ok(tags);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Tags>> PatchAsync(long key, Delta<Tags> delta)
    {
        var tags = await ctx.Tags.Include(x => x.Recipes).FirstOrDefaultAsync(x => x.Id == key);

        if (tags == null)
        {
            return NotFound();
        }

        delta.Patch(tags);

        await ctx.SaveChangesAsync();

        return Ok(tags);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var tags = await ctx.Tags.FindAsync(key);

        if (tags != null)
        {
            ctx.Tags.Remove(tags);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}

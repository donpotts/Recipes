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
public class ReviewsController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Reviews>> Get()
    {
        return Ok(ctx.Reviews);
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Reviews>> GetAsync(long key)
    {
        var reviews = await ctx.Reviews.FirstOrDefaultAsync(x => x.Id == key);

        if (reviews == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(reviews);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Reviews>> PostAsync(Reviews reviews)
    {
        var record = await ctx.Reviews.FindAsync(reviews.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        await ctx.Reviews.AddAsync(reviews);

        await ctx.SaveChangesAsync();

        return Created($"/reviews/{reviews.Id}", reviews);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Reviews>> PutAsync(long key, Reviews update)
    {
        var reviews = await ctx.Reviews.FirstOrDefaultAsync(x => x.Id == key);

        if (reviews == null)
        {
            return NotFound();
        }

        ctx.Entry(reviews).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(reviews);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Reviews>> PatchAsync(long key, Delta<Reviews> delta)
    {
        var reviews = await ctx.Reviews.FirstOrDefaultAsync(x => x.Id == key);

        if (reviews == null)
        {
            return NotFound();
        }

        delta.Patch(reviews);

        await ctx.SaveChangesAsync();

        return Ok(reviews);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var reviews = await ctx.Reviews.FindAsync(key);

        if (reviews != null)
        {
            ctx.Reviews.Remove(reviews);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}

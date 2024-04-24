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
public class UnitsController(ApplicationDbContext ctx) : ControllerBase
{
    [HttpGet("")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IQueryable<Units>> Get()
    {
        return Ok(ctx.Units);
    }

    [HttpGet("{key}")]
    [EnableQuery]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Units>> GetAsync(long key)
    {
        var units = await ctx.Units.FirstOrDefaultAsync(x => x.Id == key);

        if (units == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(units);
        }
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Units>> PostAsync(Units units)
    {
        var record = await ctx.Units.FindAsync(units.Id);
        if (record != null)
        {
            return Conflict();
        }
    
        await ctx.Units.AddAsync(units);

        await ctx.SaveChangesAsync();

        return Created($"/units/{units.Id}", units);
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Units>> PutAsync(long key, Units update)
    {
        var units = await ctx.Units.FirstOrDefaultAsync(x => x.Id == key);

        if (units == null)
        {
            return NotFound();
        }

        ctx.Entry(units).CurrentValues.SetValues(update);

        await ctx.SaveChangesAsync();

        return Ok(units);
    }

    [HttpPatch("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Units>> PatchAsync(long key, Delta<Units> delta)
    {
        var units = await ctx.Units.FirstOrDefaultAsync(x => x.Id == key);

        if (units == null)
        {
            return NotFound();
        }

        delta.Patch(units);

        await ctx.SaveChangesAsync();

        return Ok(units);
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(long key)
    {
        var units = await ctx.Units.FindAsync(key);

        if (units != null)
        {
            ctx.Units.Remove(units);
            await ctx.SaveChangesAsync();
        }

        return NoContent();
    }
}

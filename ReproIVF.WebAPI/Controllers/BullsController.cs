using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReproIVF.Shared.Entities;
using ReproIVF.Shared.Security;
using ReproIVF.WebAPI.Data;
using ReproIVF.WebAPI.Models;

namespace ReproIVF.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class BullsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BullsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Bull>>> GetAll()
    {
        var items = await _db.Bulls.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Bull>> Create(BullUpsert dto)
    {
        var entity = new Bull { Name = dto.Name, Code = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code };
        _db.Bulls.Add(entity);
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, BullUpsert dto)
    {
        var entity = await _db.Bulls.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        entity.Code = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _db.Bulls.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Bulls.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}



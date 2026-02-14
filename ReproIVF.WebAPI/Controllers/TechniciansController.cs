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
public class TechniciansController : ControllerBase
{
    private readonly AppDbContext _db;

    public TechniciansController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Technician>>> GetAll()
    {
        var items = await _db.Technicians.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Technician>> Create(NameUpsert dto)
    {
        var entity = new Technician { Name = dto.Name };
        _db.Technicians.Add(entity);
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, NameUpsert dto)
    {
        var entity = await _db.Technicians.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _db.Technicians.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Technicians.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}



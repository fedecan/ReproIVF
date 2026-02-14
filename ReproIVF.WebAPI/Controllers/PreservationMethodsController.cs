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
public class PreservationMethodsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PreservationMethodsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<PreservationMethod>>> GetAll()
    {
        var items = await _db.PreservationMethods.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<PreservationMethod>> Create(NameUpsert dto)
    {
        var entity = new PreservationMethod { Name = dto.Name };
        _db.PreservationMethods.Add(entity);
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, NameUpsert dto)
    {
        var entity = await _db.PreservationMethods.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _db.PreservationMethods.FindAsync(id);
        if (entity is null) return NotFound();

        _db.PreservationMethods.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}



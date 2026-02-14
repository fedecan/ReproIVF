using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReproIVF.WebAPI.Data;
using ReproIVF.WebAPI.Models;
using ProgramEntity = ReproIVF.Shared.Entities.Program;
using ReproIVF.Shared.Security;

namespace ReproIVF.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class ProgramsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProgramsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProgramEntity>>> GetAll()
    {
        var items = await _db.Programs.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ProgramEntity>> Create(NameUpsert dto)
    {
        var entity = new ProgramEntity { Name = dto.Name };
        _db.Programs.Add(entity);
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, NameUpsert dto)
    {
        var entity = await _db.Programs.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _db.Programs.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Programs.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}



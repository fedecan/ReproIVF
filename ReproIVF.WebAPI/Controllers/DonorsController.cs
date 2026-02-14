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
public class DonorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DonorsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Donor>>> GetAll()
    {
        var items = await _db.Donors
            .AsNoTracking()
            .Include(x => x.Program)
            .OrderBy(x => x.Code)
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Donor>> Create(DonorUpsert dto)
    {
        var entity = new Donor
        {
            Code = dto.Code,
            OwnerName = string.IsNullOrWhiteSpace(dto.OwnerName) ? null : dto.OwnerName,
            ProgramId = dto.ProgramId,
            EmbryoCount = dto.EmbryoCount
        };

        _db.Donors.Add(entity);
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, DonorUpsert dto)
    {
        var entity = await _db.Donors.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Code = dto.Code;
        entity.OwnerName = string.IsNullOrWhiteSpace(dto.OwnerName) ? null : dto.OwnerName;
        entity.ProgramId = dto.ProgramId;
        entity.EmbryoCount = dto.EmbryoCount;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entity = await _db.Donors.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Donors.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}



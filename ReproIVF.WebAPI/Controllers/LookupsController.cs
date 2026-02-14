using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReproIVF.Shared.Security;
using ReproIVF.WebAPI.Data;

namespace ReproIVF.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin + "," + AppRoles.Client)]
public class LookupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public LookupsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("programs")]
    public async Task<ActionResult> GetPrograms()
    {
        var items = await _db.Programs
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("clients")]
    public async Task<ActionResult> GetClients()
    {
        var items = await _db.Clients
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("donors")]
    public async Task<ActionResult> GetDonors()
    {
        var items = await _db.Donors
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("bulls")]
    public async Task<ActionResult> GetBulls()
    {
        var items = await _db.Bulls
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("technicians")]
    public async Task<ActionResult> GetTechnicians()
    {
        var items = await _db.Technicians
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("semen-types")]
    public async Task<ActionResult> GetSemenTypes()
    {
        var items = await _db.SemenTypes
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("preservation-methods")]
    public async Task<ActionResult> GetPreservationMethods()
    {
        var items = await _db.PreservationMethods
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(items);
    }
}


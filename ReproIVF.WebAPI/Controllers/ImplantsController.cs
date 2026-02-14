using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReproIVF.WebAPI.Export;
using ReproIVF.Shared.Entities;
using ReproIVF.Shared.Security;
using ReproIVF.WebAPI.Data;
using ReproIVF.WebAPI.Models;

namespace ReproIVF.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin + "," + AppRoles.Client)]
public class ImplantsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ImplantsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Implant>>> GetAll()
    {
        var implants = await _db.Implants
            .Include(i => i.Program)
            .Include(i => i.Owner)
            .Include(i => i.Donor)
            .Include(i => i.Bull)
            .Include(i => i.SemenType)
            .Include(i => i.PreservationMethod)
            .Include(i => i.Technician)
            .AsNoTracking()
            .ToListAsync();

        return Ok(implants);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Implant>> GetById(int id)
    {
        var implant = await _db.Implants
            .Include(i => i.Program)
            .Include(i => i.Owner)
            .Include(i => i.Donor)
            .Include(i => i.Bull)
            .Include(i => i.SemenType)
            .Include(i => i.PreservationMethod)
            .Include(i => i.Technician)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return implant is null ? NotFound() : Ok(implant);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<Implant>> Create(ImplantCreateUpdate dto)
    {
        ApplyDefaults(dto);

        var implant = new Implant
        {
            ProgramId = dto.ProgramId,
            OwnerId = dto.OwnerId,
            OpuDate = dto.OpuDate,
            FertDate = dto.FertDate,
            FreezingDate = dto.FreezingDate,
            StrawId = dto.StrawId,
            DonorId = dto.DonorId,
            BullId = dto.BullId,
            SemenTypeId = dto.SemenTypeId,
            PreservationMethodId = dto.PreservationMethodId,
            ImplantDate = dto.ImplantDate,
            RecipId = dto.RecipId,
            TechnicianId = dto.TechnicianId,
            InCalf = dto.InCalf
        };

        _db.Implants.Add(implant);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = implant.Id }, implant);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult> Update(int id, ImplantCreateUpdate dto)
    {
        var implant = await _db.Implants.FindAsync(id);
        if (implant is null)
        {
            return NotFound();
        }

        ApplyDefaults(dto);

        implant.ProgramId = dto.ProgramId;
        implant.OwnerId = dto.OwnerId;
        implant.OpuDate = dto.OpuDate;
        implant.FertDate = dto.FertDate;
        implant.FreezingDate = dto.FreezingDate;
        implant.StrawId = dto.StrawId;
        implant.DonorId = dto.DonorId;
        implant.BullId = dto.BullId;
        implant.SemenTypeId = dto.SemenTypeId;
        implant.PreservationMethodId = dto.PreservationMethodId;
        implant.ImplantDate = dto.ImplantDate;
        implant.RecipId = dto.RecipId;
        implant.TechnicianId = dto.TechnicianId;
        implant.InCalf = dto.InCalf;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult> Delete(int id)
    {
        var implant = await _db.Implants.FindAsync(id);
        if (implant is null)
        {
            return NotFound();
        }

        _db.Implants.Remove(implant);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("export")]
    public async Task<ActionResult> Export(ImplantExportRequest? request)
    {
        var query = _db.Implants
            .Include(i => i.Program)
            .Include(i => i.Owner)
            .Include(i => i.Donor)
            .Include(i => i.Bull)
            .Include(i => i.SemenType)
            .Include(i => i.PreservationMethod)
            .Include(i => i.Technician)
            .AsNoTracking();

        if (request?.Ids is { Count: > 0 })
        {
            query = query.Where(i => request.Ids.Contains(i.Id));
        }

        var implants = await query.OrderBy(i => i.Id).ToListAsync();

        var content = XlsxExportBuilder.BuildImplantsWorkbook(implants);
        var fileName = $"implants-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    private static void ApplyDefaults(ImplantCreateUpdate dto)
    {
        if (dto.OpuDate.HasValue)
        {
            if (!dto.FertDate.HasValue)
            {
                dto.FertDate = dto.OpuDate.Value.AddDays(1);
            }

            if (!dto.FreezingDate.HasValue)
            {
                dto.FreezingDate = dto.OpuDate.Value.AddDays(8);
            }
        }
    }
}


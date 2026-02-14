using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReproIVF.Shared.Auth;
using ReproIVF.Shared.Entities;
using ReproIVF.Shared.Security;
using ReproIVF.WebAPI.Data;
using ReproIVF.WebAPI.Security;

namespace ReproIVF.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserView>>> GetAll()
    {
        var users = await _db.Users
            .AsNoTracking()
            .OrderBy(x => x.Username)
            .Select(x => new UserView
            {
                Id = x.Id,
                Username = x.Username,
                Role = x.Role
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserView>> Create(UserUpsert dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Password is required.");
        }

        if (!IsValidRole(dto.Role))
        {
            return BadRequest("Invalid role.");
        }

        var username = dto.Username.Trim();
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username is required.");
        }

        if (await _db.Users.AnyAsync(x => x.Username == username))
        {
            return Conflict("Username already exists.");
        }

        var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
        var normalizedRole = NormalizeRole(dto.Role);
        var user = new AppUser
        {
            Username = username,
            Role = normalizedRole,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new UserView { Id = user.Id, Username = user.Username, Role = user.Role });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, UserUpsert dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        if (!IsValidRole(dto.Role))
        {
            return BadRequest("Invalid role.");
        }

        var username = dto.Username.Trim();
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Username is required.");
        }

        var exists = await _db.Users.AnyAsync(x => x.Username == username && x.Id != id);
        if (exists)
        {
            return Conflict("Username already exists.");
        }

        user.Username = username;
        user.Role = NormalizeRole(dto.Role);

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static bool IsValidRole(string role) =>
        string.Equals(role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(role, AppRoles.Client, StringComparison.OrdinalIgnoreCase);

    private static string NormalizeRole(string role) =>
        string.Equals(role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase)
            ? AppRoles.Admin
            : AppRoles.Client;
}


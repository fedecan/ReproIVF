using System.ComponentModel.DataAnnotations;

namespace ReproIVF.Shared.Entities;

public class Donor
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? OwnerName { get; set; }

    public int? ProgramId { get; set; }
    public Program? Program { get; set; }

    public int? EmbryoCount { get; set; }

    public ICollection<Implant> Implants { get; set; } = new List<Implant>();
}

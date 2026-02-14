using System.ComponentModel.DataAnnotations;

namespace ReproIVF.Shared.Entities;

public class Bull
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Code { get; set; }

    public ICollection<Implant> Implants { get; set; } = new List<Implant>();
}

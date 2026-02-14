using System.ComponentModel.DataAnnotations;

namespace ReproIVF.Shared.Entities;

public class Technician
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Implant> Implants { get; set; } = new List<Implant>();
}

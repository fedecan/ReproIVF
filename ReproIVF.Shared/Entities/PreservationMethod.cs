using System.ComponentModel.DataAnnotations;

namespace ReproIVF.Shared.Entities;

public class PreservationMethod
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Implant> Implants { get; set; } = new List<Implant>();
}

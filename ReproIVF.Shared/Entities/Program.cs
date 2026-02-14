using System.ComponentModel.DataAnnotations;

namespace ReproIVF.Shared.Entities;

public class Program
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Implant> Implants { get; set; } = new List<Implant>();
    public ICollection<Donor> Donors { get; set; } = new List<Donor>();
}

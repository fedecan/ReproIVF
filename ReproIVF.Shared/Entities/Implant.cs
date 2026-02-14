using System.ComponentModel.DataAnnotations;

namespace ReproIVF.Shared.Entities;

public class Implant
{
    public int Id { get; set; }

    public int? ProgramId { get; set; }
    public Program? Program { get; set; }

    public int? OwnerId { get; set; }
    public Client? Owner { get; set; }

    public DateTime? OpuDate { get; set; }
    public DateTime? FertDate { get; set; }
    public DateTime? FreezingDate { get; set; }

    public int? StrawId { get; set; }

    public int? DonorId { get; set; }
    public Donor? Donor { get; set; }

    public int? BullId { get; set; }
    public Bull? Bull { get; set; }

    public int? SemenTypeId { get; set; }
    public SemenType? SemenType { get; set; }

    public int? PreservationMethodId { get; set; }
    public PreservationMethod? PreservationMethod { get; set; }

    public DateTime? ImplantDate { get; set; }

    [MaxLength(100)]
    public string? RecipId { get; set; }

    public int? TechnicianId { get; set; }
    public Technician? Technician { get; set; }

    public bool? InCalf { get; set; }
}

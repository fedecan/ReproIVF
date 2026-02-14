namespace ReproIVF.Client.Models;

public class ImplantUpsert
{
    public int? ProgramId { get; set; }
    public int? OwnerId { get; set; }
    public DateTime? OpuDate { get; set; }
    public DateTime? FertDate { get; set; }
    public DateTime? FreezingDate { get; set; }
    public int? StrawId { get; set; }
    public int? DonorId { get; set; }
    public int? BullId { get; set; }
    public int? SemenTypeId { get; set; }
    public int? PreservationMethodId { get; set; }
    public DateTime? ImplantDate { get; set; }
    public string? RecipId { get; set; }
    public int? TechnicianId { get; set; }
    public bool? InCalf { get; set; }
}

namespace ReproIVF.Client.Models;

public class ImplantFilters
{
    public int? ProgramId { get; set; }
    public int? OwnerId { get; set; }
    public int? DonorId { get; set; }
    public int? BullId { get; set; }
    public int? TechnicianId { get; set; }
    public int? SemenTypeId { get; set; }
    public int? PreservationMethodId { get; set; }
    public bool? InCalf { get; set; }
    public string? RecipId { get; set; }
}

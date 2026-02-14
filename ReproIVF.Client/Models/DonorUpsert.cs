namespace ReproIVF.Client.Models;

public class DonorUpsert
{
    public string Code { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
    public int? ProgramId { get; set; }
    public int? EmbryoCount { get; set; }
}

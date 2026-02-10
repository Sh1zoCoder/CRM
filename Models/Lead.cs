namespace CRM.Models;

public enum LeadStatus
{
    New,
    InProgress,
    Won,
    Lost
}

public class Lead
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Phone { get; set; } = "";

    public string? Email { get; set; }

    public string Source { get; set; } = "unknown";

    public LeadStatus Status { get; set; } = LeadStatus.New;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
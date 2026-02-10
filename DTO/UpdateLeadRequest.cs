using System.ComponentModel.DataAnnotations;
using CRM.Models;

namespace CRM.DTO;

public class UpdateLeadRequest
{
    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    [Required, StringLength(50)]
    public string Phone { get; set; } = "";

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string Source { get; set; } = "unknown";

    public LeadStatus Status { get; set; } = LeadStatus.New;
}
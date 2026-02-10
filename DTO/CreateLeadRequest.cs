using System.ComponentModel.DataAnnotations;

namespace CRM.DTO;

public class CreateLeadRequest
{
    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    [Required, StringLength(50)]
    public string Phone { get; set; } = "";

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string Source { get; set; } = "unknown";
}
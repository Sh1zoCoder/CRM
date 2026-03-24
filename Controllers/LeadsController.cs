using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CRM.DTO;
using CRM.Models;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.Data;
using CRM.Models;

namespace JsonFileApi.Controllers;


[ApiController]
[Route("api/leads")]
public class LeadsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<LeadsController> _logger;
    public LeadsController(ILogger<LeadsController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET /api/leads?status=New&query=ivan
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? status, [FromQuery] string? query)
    {
        IQueryable<Lead> itemsQuery = _context.Leads.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<LeadStatus>(status, true, out var parsedStatus))
        {
            itemsQuery = itemsQuery.Where(lead => lead.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim().ToLower();

            itemsQuery = itemsQuery.Where(lead =>
                lead.Name.ToLower().Contains(normalizedQuery) ||
                lead.Phone.ToLower().Contains(normalizedQuery) ||
                (lead.Email != null && lead.Email.ToLower().Contains(normalizedQuery)) ||
                lead.Source.ToLower().Contains(normalizedQuery));
        }

        var items = itemsQuery.ToList();

        return Ok(items); // C# -> JSON (сериализация ответа)
    }

    // GET /api/leads/5
    [HttpGet("{id:int}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var item = _context.Leads.FirstOrDefault(x => x.Id == id);

        if (item is null)
        {
            return NotFound(new { message = $"Lead with id={id} not found" });
        }

        return Ok(item);
    }

    // POST /api/leads
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeadRequest request)
    {
        // JSON -> C# (десериализация тела запроса)
        Lead lead = new Lead
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Source = string.IsNullOrWhiteSpace(request.Source) ? "unknown" : request.Source.Trim(),
            Status = LeadStatus.New
        };

        _context.Leads.Add(lead);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = lead.Id }, lead);
    }

    [HttpPost("test")]
    public IActionResult Create()
    {
        try
        {
            throw new Exception("Произошла ошибка");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception: {Message}", exception.Message);
        }

        return Ok(); ;
    }

    // PUT /api/leads/5
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateLeadRequest request)
    {
        var lead = _context.Leads.FirstOrDefault(x => x.Id == id)!;

        if (lead is null)
        {
            return NotFound(new { message = $"Lead with id={id} not found" });
        }

        lead.Name = request.Name.Trim();
        lead.Phone = request.Phone.Trim();
        lead.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        lead.Source = string.IsNullOrWhiteSpace(request.Source) ? "unknown" : request.Source.Trim();
        lead.Status = request.Status;

        _context.SaveChanges();

        return Ok(lead);
    }

    // DELETE /api/leads/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var lead = _context.Leads.FirstOrDefault(x => x.Id == id);

        if (lead is null)
        {
            return NotFound(new { message = $"Lead with id={id} not found" });
        }

        _context.Leads.Remove(lead);
        _context.SaveChanges();

        return NoContent();
    }
}

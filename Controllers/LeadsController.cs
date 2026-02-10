using Microsoft.AspNetCore.Mvc;
using CRM.DTO;
using CRM.Models;
using CRM.Storage;

namespace JsonFileApi.Controllers;

[ApiController]
[Route("api/leads")]
public class LeadsController : ControllerBase
{
    private readonly JsonFileLeadStore _store;

    public LeadsController(JsonFileLeadStore store)
    {
        _store = store;
    }

    // GET /api/leads?status=New&query=ivan
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? status, [FromQuery] string? query)
    {
        var items = _store.GetAll(status, query);
        return Ok(items); // C# -> JSON (сериализация ответа)
    }

    // GET /api/leads/5
    [HttpGet("{id:int}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var item = _store.GetById(id);
        if (item is null)
        {
            return NotFound(new { message = $"Lead with id={id} not found" });
        }
        return Ok(item);
    }

    // POST /api/leads
    [HttpPost]
    public IActionResult Create([FromBody] CreateLeadRequest request)
    {
        // JSON -> C# (десериализация тела запроса)
        var lead = new Lead
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Source = string.IsNullOrWhiteSpace(request.Source) ? "unknown" : request.Source.Trim(),
            Status = LeadStatus.New
        };

        var created = _store.Add(lead);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/leads/5
    [HttpPut("{id:int}")]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateLeadRequest request)
    {
        var ok = _store.Update(id, lead =>
        {
            lead.Name = request.Name.Trim();
            lead.Phone = request.Phone.Trim();
            lead.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            lead.Source = string.IsNullOrWhiteSpace(request.Source) ? "unknown" : request.Source.Trim();
            lead.Status = request.Status;
        }, out var updated);

        if (!ok)
        return NotFound(new { message = $"Lead with id={id} not found" });

        return Ok(updated);
    }

    // DELETE /api/leads/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var ok = _store.Delete(id);
        if (!ok)
        return NotFound(new { message = $"Lead with id={id} not found" });

        return NoContent();
    }  
}
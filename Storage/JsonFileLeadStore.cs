using System.Text.Json;
using CRM.Models;

namespace CRM.Storage;

public class JsonFileLeadStore
{
private readonly string _filePath;
private readonly object _lock = new();

private List<Lead> _items = new();
private int _nextId = 1;
private bool _loaded = false;

public JsonFileLeadStore(IWebHostEnvironment env)
{
    var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
    Directory.CreateDirectory(dataDir);

    _filePath = Path.Combine(dataDir, "leads.json");
    }

        private void EnsureLoaded()
        {
            if (_loaded) return;

            lock (_lock)
            {
                if (_loaded) return;

                if (!File.Exists(_filePath))
                {
                    _items = new List<Lead>();
                    _nextId = 1;
                    _loaded = true;
                    return;
                }

                var json = File.ReadAllText(_filePath);
                _items = string.IsNullOrWhiteSpace(json)
                ? new List<Lead>()
                : (JsonSerializer.Deserialize<List<Lead>>(json) ?? new List<Lead>());

                _nextId = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1;
                _loaded = true;
            }

        }

        private void Save()
        {
            var tmp = _filePath + ".tmp";
            var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            File.WriteAllText(tmp, json);
            File.Copy(tmp, _filePath, overwrite: true);
            File.Delete(tmp);
        }

        public IReadOnlyList<Lead> GetAll(string? status, string? query)
        {
            EnsureLoaded();
            lock (_lock)
            {
                IEnumerable<Lead> result = _items;

                if (!string.IsNullOrWhiteSpace(status)
                && Enum.TryParse<LeadStatus>(status, ignoreCase: true, out var st))
                {
                    result = result.Where(x => x.Status == st);
                }

                if (!string.IsNullOrWhiteSpace(query))
                {
                    var q = query.Trim().ToLowerInvariant();
                    result = result.Where(x =>
                    x.Name.ToLowerInvariant().Contains(q) ||
                    x.Phone.ToLowerInvariant().Contains(q) ||
                    (x.Email?.ToLowerInvariant().Contains(q) ?? false));
                }

                // Для CRM обычно удобно: новые сверху
                return result
                .OrderBy(x => x.Status) // New раньше
                .ThenByDescending(x => x.UpdatedAtUtc)
                .ToList();
            }
        }

        public Lead? GetById(int id)
        {
            EnsureLoaded();
            lock (_lock) return _items.FirstOrDefault(x => x.Id == id);
        }

        public Lead Add(Lead lead)
        {
            EnsureLoaded();
            lock (_lock)
            {
                lead.Id = _nextId++;
                lead.CreatedAtUtc = DateTime.UtcNow;
                lead.UpdatedAtUtc = lead.CreatedAtUtc;

                _items.Add(lead);
                Save();
                return lead;
            }
        }

        public bool Update(int id, Action<Lead> apply, out Lead? updated)
        {
            EnsureLoaded();
            lock (_lock)
            {
                var item = _items.FirstOrDefault(x => x.Id == id);
                if (item is null)
                {
                    updated = null;
                    return false;
                }

                apply(item);
                item.UpdatedAtUtc = DateTime.UtcNow;

                Save();
                updated = item;
                return true;
            }
        }

        public bool Delete(int id)
        {
            EnsureLoaded();
            lock (_lock)
            {
            var removed = _items.RemoveAll(x => x.Id == id) > 0;
            if (!removed) return false;

            Save();
            return true;
        }
    }
}
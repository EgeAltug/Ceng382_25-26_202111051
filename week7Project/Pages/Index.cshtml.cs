using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using week7Project.Models;
using week7Project.Utils;

namespace week7Project.Pages;

public class IndexModel : PageModel
{
    private static readonly List<ClassInformationModel> _classes = new();
    private static int _nextId = 1;
    private const int PageSize = 10;

    [BindProperty(SupportsGet = true)]
    public string? FilterClassName { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public List<ClassInformationTableModel> PaginatedClasses { get; set; } = new();

    public int TotalPages { get; set; }

    [BindProperty]
    public ClassInformationModel Input { get; set; } = new();

    [TempData]
    public int? EditId { get; set; }

    public void OnGet()
    {
        if (_classes.Count < 100)
        {
            for (int i = _classes.Count; i < 100; i++)
            {
                _classes.Add(new ClassInformationModel
                {
                    Id = _nextId++,
                    ClassName = $"Class {i + 1}",
                    StudentCount = 10,
                    Description = $"Description for Class {i + 1}"
                });
            }
        }

        var query = _classes.AsQueryable();
        if (!string.IsNullOrWhiteSpace(FilterClassName))
        {
            query = query.Where(c => c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query.ToList();

        TotalPages = (int)Math.Ceiling(filtered.Count / (double)PageSize);
        PaginatedClasses = filtered
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(c => new ClassInformationTableModel
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            })
            .ToList();

        if (EditId.HasValue)
        {
            Input = _classes.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
        }
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid) return Page();
        Input.Id = _nextId++;
        _classes.Add(Input);
        Input = new ClassInformationModel();
        return RedirectToPage(new { FilterClassName, PageNumber });
    }

    public IActionResult OnPostEdit(int id)
    {
        EditId = id;
        return RedirectToPage(new { FilterClassName, PageNumber });
    }

    public IActionResult OnPostUpdate()
    {
        if (!ModelState.IsValid) return Page();

        var existing = _classes.FirstOrDefault(c => c.Id == Input.Id);
        if (existing != null)
        {
            existing.ClassName = Input.ClassName;
            existing.StudentCount = Input.StudentCount;
            existing.Description = Input.Description;
        }
        Input = new ClassInformationModel();
        EditId = null;
        return RedirectToPage(new { FilterClassName, PageNumber });
    }

    public IActionResult OnPostDelete(int id)
    {
        var item = _classes.FirstOrDefault(c => c.Id == id);
        if (item != null) _classes.Remove(item);
        return RedirectToPage(new { FilterClassName, PageNumber });
    }

    public IActionResult OnPostExport(string exportType, string? selectedColumns)
    {
        var baseData = exportType == "filtered" 
            ? GetFilteredData() 
            : _classes;

        var columnIndices = selectedColumns?.Split(',', StringSplitOptions.RemoveEmptyEntries) 
            ?? Array.Empty<string>();

        var properties = GetExportProperties(columnIndices);
        var exportData = baseData.Select(c => new ClassInformationTableModel
        {
            Id = c.Id,
            ClassName = c.ClassName,
            StudentCount = c.StudentCount,
            Description = c.Description
        });

        var json = JsonExporter.Instance.Export(exportData, properties);

        return new FileContentResult(Encoding.UTF8.GetBytes(json), "application/json")
        {
            FileDownloadName = $"classes-{exportType}-{DateTime.Now:yyyyMMddHHmmss}.json"
        };
    }

    private List<string> GetExportProperties(string[] indices)
    {
        var validIndices = indices
            .Where(i => int.TryParse(i, out _))
            .Select(int.Parse)
            .Where(i => i >= 0 && i < 3)
            .Distinct();

        var properties = new List<string>();
        foreach (var index in validIndices)
        {
            properties.Add(index switch
            {
                0 => "ClassName",
                1 => "StudentCount",
                2 => "Description",
                _ => throw new ArgumentOutOfRangeException()
            });
        }
        return properties;
    }

    private List<ClassInformationModel> GetFilteredData()
    {
        return _classes
            .Where(c => string.IsNullOrWhiteSpace(FilterClassName) || 
                c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
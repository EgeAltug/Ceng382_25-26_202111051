using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using week7Project.Models;
using week7Project.Utils; // Assuming Utils.cs is in the week7Project.Utils namespace
using System.Text.Json;

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

    // Bind the SelectedColumns from the export form.
    [BindProperty]
    public string[]? SelectedColumns { get; set; }

    // Bind export mode ("filtered" or "unfiltered")
    [BindProperty]
    public string? ExportMode { get; set; }

    public void OnGet()
    {
        // Generate synthetic data only once
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

        // Filter the list based on the filter input
        var query = _classes.AsQueryable();
        if (!string.IsNullOrWhiteSpace(FilterClassName))
        {
            query = query.Where(c => c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query.ToList();

        // Pagination
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

        // If editing, load the record into the Input model so that the form fields are pre-populated.
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
        // Set EditId and reload the page so OnGet can prefill the form.
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
        // Clear the edit state
        Input = new ClassInformationModel();
        EditId = null;
        return RedirectToPage(new { FilterClassName, PageNumber });
    }

    public IActionResult OnPostDelete(int id)
    {
        var item = _classes.FirstOrDefault(c => c.Id == id);
        if (item != null)
        {
            _classes.Remove(item);
        }
        return RedirectToPage(new { FilterClassName, PageNumber });
    }

    // New Export Handler
    public IActionResult OnPostExport()
    {
        // Determine which records to export based on ExportMode.
        List<ClassInformationModel> exportData;
        if (ExportMode == "filtered")
        {
            // Apply the same filtering as in OnGet.
            var query = _classes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
            }
            exportData = query.ToList();
        }
        else // "unfiltered" or any other case
        {
            exportData = _classes;
        }

        // Use the Utils singleton to export data to JSON.
        string jsonResult = Utils.Instance.ExportToJson(exportData, SelectedColumns?.ToList());

        // Return the JSON as a downloadable file.
        return File(System.Text.Encoding.UTF8.GetBytes(jsonResult), "application/json", "Export.json");
    }
}

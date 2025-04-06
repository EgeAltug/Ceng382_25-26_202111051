using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using week6Project.Models;
using System.Linq;

namespace week6Project.Pages;

public class IndexModel : PageModel
{
    private static readonly List<ClassInformationModel> _classes = GenerateSampleData();
    private static int _nextId = 101;

    [BindProperty(SupportsGet = true)]
    public ClassInformationTable TableData { get; set; } = new();

    [BindProperty]
    public ClassInformationModel Input { get; set; } = new();

    [TempData]
    public int? EditId { get; set; }

    public void OnGet()
    {
        // Initialize edit mode if needed
        if (EditId.HasValue)
        {
            Input = _classes.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
        }

        // Apply filtering and pagination
        var query = _classes.AsQueryable();
        
        if (!string.IsNullOrEmpty(TableData.ClassNameFilter))
            query = query.Where(c => c.ClassName.Contains(TableData.ClassNameFilter));
        
        if (!string.IsNullOrEmpty(TableData.DescriptionFilter))
            query = query.Where(c => c.Description.Contains(TableData.DescriptionFilter));

        TableData.TotalPages = (int)Math.Ceiling(query.Count() / (double)TableData.PageSize);
        TableData.Items = query
            .Skip((TableData.CurrentPage - 1) * TableData.PageSize)
            .Take(TableData.PageSize)
            .ToList();
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid) return Page();

        Input.Id = _nextId++;
        _classes.Add(Input);
        
        return RedirectWithState();
    }

    public IActionResult OnPostEdit(int id)
    {
        EditId = id;
        return RedirectWithState();
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
        
        return RedirectWithState();
    }

    public IActionResult OnPostDelete(int id)
    {
        var item = _classes.FirstOrDefault(c => c.Id == id);
        if (item != null) _classes.Remove(item);
        
        return RedirectWithState();
    }

    private IActionResult RedirectWithState()
    {
        return RedirectToPage(new {
            currentpage = TableData.CurrentPage,
            classnamefilter = TableData.ClassNameFilter,
            descriptionfilter = TableData.DescriptionFilter,
            pagesize = TableData.PageSize
        });
    }

    private static List<ClassInformationModel> GenerateSampleData()
    {
        var sampleData = new List<ClassInformationModel>();
        var random = new Random();
        
        for (int i = 1; i <= 100; i++)
        {
            sampleData.Add(new ClassInformationModel {
                Id = i,
                ClassName = $"Class {i}",
                StudentCount = random.Next(15, 50),
                Description = $"Description for Class {i}"
            });
        }
        return sampleData;
    }
}
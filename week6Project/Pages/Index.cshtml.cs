using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using week5Project.Models;

namespace week5Project.Pages;

public class IndexModel : PageModel
{
    private static readonly List<ClassInformationModel> _classes = new();
    private static int _nextId = 1;

    [BindProperty]
    public ClassInformationModel Input { get; set; } = new();

    public IReadOnlyList<ClassInformationModel> Classes => _classes.AsReadOnly();

    [TempData]
    public int? EditId { get; set; }

    public void OnGet()
    {
        if (EditId.HasValue)
        {
            Input = _classes.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
        }
    }

    public IActionResult OnPostAdd()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Input.Id = _nextId++;
        _classes.Add(Input);
        Input = new ClassInformationModel();
        return RedirectToPage();
    }

    public IActionResult OnPostEdit(int id)
    {
        EditId = id;
        return RedirectToPage();
    }

    public IActionResult OnPostUpdate()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existing = _classes.FirstOrDefault(c => c.Id == Input.Id);
        if (existing != null)
        {
            existing.ClassName = Input.ClassName;
            existing.StudentCount = Input.StudentCount;
            existing.Description = Input.Description;
        }
        Input = new ClassInformationModel();
        EditId = null;
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var item = _classes.FirstOrDefault(c => c.Id == id);
        if (item != null)
        {
            _classes.Remove(item);
        }
        return RedirectToPage();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using week5Project.Models;
using System.Collections.Generic;
using System.Linq;

namespace week5Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        // Static list acting as an in-memory database
        private static List<ClassInformationModel> ClassList = new List<ClassInformationModel>();
        private static int _nextId = 1;

        // Property for form data
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; }

        // GET request to load page and display current data
        public void OnGet()
        {
            // No additional logic needed for now, just show the class list
        }

        // POST request to handle adding a new class
        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Assign the next ID and add to the list
            ClassInfo.Id = _nextId++;
            ClassList.Add(ClassInfo);

            // Clear form after adding
            ClassInfo = new ClassInformationModel();

            return RedirectToPage(); // Refresh the page
        }

        // Edit an existing class entry (get current data to pre-fill form)
        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassInfo = classToEdit;
            }

            return Page();
        }

        // POST request to update an existing class
        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var classToUpdate = ClassList.FirstOrDefault(c => c.Id == ClassInfo.Id);
            if (classToUpdate != null)
            {
                classToUpdate.ClassName = ClassInfo.ClassName;
                classToUpdate.StudentCount = ClassInfo.StudentCount;
                classToUpdate.Description = ClassInfo.Description;
            }

            return RedirectToPage(); // Refresh the page after updating
        }

        // POST request to delete a class entry
        public IActionResult OnPostDelete(int id)
        {
            var classToDelete = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                ClassList.Remove(classToDelete);
            }

            return RedirectToPage(); // Refresh the page after deletion
        }

        // Property to get the class list for displaying in the table
        public List<ClassInformationModel> Classes => ClassList;
    }
}

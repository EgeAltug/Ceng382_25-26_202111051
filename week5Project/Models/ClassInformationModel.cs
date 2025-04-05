using System.ComponentModel.DataAnnotations;

namespace week5Project.Models;

public class ClassInformationModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Class Name is required")]
    [Display(Name = "Class Name")]
    public string ClassName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Student Count is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Student Count must be at least 1")]
    [Display(Name = "Student Count")]
    public int StudentCount { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;
}
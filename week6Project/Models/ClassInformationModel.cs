namespace week6Project.Models;

public class ClassInformationTable
{
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public string? ClassNameFilter { get; set; }
    public string? DescriptionFilter { get; set; }
    public List<ClassInformationModel> Items { get; set; } = new();
}
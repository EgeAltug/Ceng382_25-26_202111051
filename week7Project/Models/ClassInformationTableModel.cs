namespace week7Project.Models;
public class ClassInformationTableModel
{
    public int Id { get; set; }  // Used in background
    public string ClassName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public string Description { get; set; } = string.Empty;
}

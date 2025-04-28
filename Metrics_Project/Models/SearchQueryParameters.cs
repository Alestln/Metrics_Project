namespace Metrics_Project.Models;

public class SearchQueryParameters
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public int SortDescending { get; set; } = 0;
}
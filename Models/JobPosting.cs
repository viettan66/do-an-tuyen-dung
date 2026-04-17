namespace JobBoard.Models;

public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string FullDescription { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }

    // Thêm các property cho trang chi tiết
    public string Type { get; set; } = string.Empty; // Full-time, Part-time, ...
    public string Department { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string>? Responsibilities { get; set; }
    public List<string>? Requirements { get; set; }
    public string Experience { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string RemoteType { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
}


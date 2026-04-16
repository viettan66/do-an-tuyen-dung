namespace JobBoard.Models;

public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string FullDescription { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
}


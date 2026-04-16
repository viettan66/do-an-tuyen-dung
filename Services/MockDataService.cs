using System.Text.Json;
using JobBoard.Models;

namespace JobBoard.Services;

public class MockDataService
{
    private readonly IWebHostEnvironment _env;
    private List<User> _users = new();
    private List<JobPosting> _jobs = new();

    public MockDataService(IWebHostEnvironment env)
    {
        _env = env;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var usersPath = Path.Combine(_env.ContentRootPath, "Data", "users.json");
            if (File.Exists(usersPath))
            {
                var js = File.ReadAllText(usersPath);
                _users = JsonSerializer.Deserialize<List<User>>(js, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>();
            }
            else
            {
                _users = new List<User> { new User { Username = "admin", Password = "password", DisplayName = "Admin User" } };
            }

            var jobsPath = Path.Combine(_env.ContentRootPath, "Data", "jobs.json");
            if (File.Exists(jobsPath))
            {
                var js = File.ReadAllText(jobsPath);
                _jobs = JsonSerializer.Deserialize<List<JobPosting>>(js, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<JobPosting>();
            }
            else
            {
                _jobs = new List<JobPosting>();
            }
        }
        catch
        {
            _users = new List<User> { new User { Username = "admin", Password = "password", DisplayName = "Admin User" } };
            _jobs = new List<JobPosting>();
        }
    }

    public User? ValidateUser(string username, string password) => _users.FirstOrDefault(u => u.Username == username && u.Password == password);

    public IEnumerable<JobPosting> GetJobs() => _jobs.OrderByDescending(j => j.PostedDate);

    public JobPosting? GetJobById(int id) => _jobs.FirstOrDefault(j => j.Id == id);
}


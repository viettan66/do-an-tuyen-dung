using System.Text.Json;
using JobBoard.Models;

namespace JobBoard.Services;

public class MockDataService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<MockDataService> _logger;
    private List<User> _users = new();
    private List<JobPosting> _jobs = new();

    public MockDataService(IWebHostEnvironment env, ILogger<MockDataService> logger)
    {
        _env = env;
        _logger = logger;
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var contentRoot = _env.ContentRootPath;
            _logger.LogInformation($"ContentRootPath: {contentRoot}");

            var usersPath = Path.Combine(contentRoot, "Data", "users.json");
            _logger.LogInformation($"Looking for users.json at: {usersPath}");
            
            if (File.Exists(usersPath))
            {
                var js = File.ReadAllText(usersPath);
                _users = JsonSerializer.Deserialize<List<User>>(js, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>();
                _logger.LogInformation($"Loaded {_users.Count} users from JSON file");
            }
            else
            {
                _logger.LogWarning($"users.json not found at {usersPath}, using fallback user");
                _users = new List<User> { new User { Username = "admin", Password = "password", DisplayName = "Admin User" } };
            }

            var jobsPath = Path.Combine(contentRoot, "Data", "jobs.json");
            _logger.LogInformation($"Looking for jobs.json at: {jobsPath}");
            
            if (File.Exists(jobsPath))
            {
                var js = File.ReadAllText(jobsPath);
                _jobs = JsonSerializer.Deserialize<List<JobPosting>>(js, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<JobPosting>();
                _logger.LogInformation($"Loaded {_jobs.Count} jobs from JSON file");
            }
            else
            {
                _logger.LogWarning($"jobs.json not found at {jobsPath}");
                _jobs = new List<JobPosting>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data files, using fallback data");
            _users = new List<User> { new User { Username = "admin", Password = "password", DisplayName = "Admin User" } };
            _jobs = new List<JobPosting>();
        }
    }

    public User? ValidateUser(string username, string password) => _users.FirstOrDefault(u => u.Username == username && u.Password == password);

    public IEnumerable<JobPosting> GetJobs() => _jobs.OrderByDescending(j => j.PostedDate);

    public JobPosting? GetJobById(int id) => _jobs.FirstOrDefault(j => j.Id == id);
}


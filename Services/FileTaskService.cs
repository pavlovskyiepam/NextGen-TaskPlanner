using System.Text.Json;
using TaskPlanner.Models;

namespace TaskPlanner.Services
{
    public class FileTaskService : ITaskService
    {
        private readonly string _filePath;
        private readonly ILogger<FileTaskService> _logger;

        public FileTaskService(ILogger<FileTaskService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _filePath = Path.Combine(environment.ContentRootPath, "Data", "tasks.json");
            
            _logger.LogInformation("FileTaskService initialized with path: {FilePath}", _filePath);
            
            // Ensure the Data directory exists
            var dataDir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir!);
                _logger.LogInformation("Created Data directory: {DataDir}", dataDir);
            }
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            try
            {
                _logger.LogInformation("Getting all tasks from: {FilePath}", _filePath);
                
                if (!File.Exists(_filePath))
                {
                    _logger.LogInformation("Tasks file does not exist, returning empty list");
                    return new List<TaskItem>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                if (string.IsNullOrEmpty(json))
                {
                    _logger.LogInformation("Tasks file is empty, returning empty list");
                    return new List<TaskItem>();
                }

                var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                _logger.LogInformation("Loaded {Count} tasks from file", tasks.Count);
                return tasks.OrderBy(t => t.DueDate).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading tasks from file");
                return new List<TaskItem>();
            }
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            var tasks = await GetAllTasksAsync();
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            _logger.LogInformation("Creating task: {Title}", task.Title);
            
            try
            {
                _logger.LogInformation("Starting task creation process");
                
                var tasks = await GetAllTasksAsync();
                _logger.LogInformation("Current tasks count: {Count}", tasks.Count);
                
                // Generate new ID
                task.Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
                task.CreatedAt = DateTime.Now;
                
                _logger.LogInformation("Generated task ID: {Id}", task.Id);
                
                tasks.Add(task);
                _logger.LogInformation("Added task to list, new count: {Count}", tasks.Count);
                
                await SaveTasksAsync(tasks);
                _logger.LogInformation("Task saved successfully");
                
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            try
            {
                var tasks = await GetAllTasksAsync();
                var existingTask = tasks.FirstOrDefault(t => t.Id == task.Id);
                
                if (existingTask == null)
                {
                    throw new InvalidOperationException("Task not found");
                }

                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.DueDate = task.DueDate;
                existingTask.IsCompleted = task.IsCompleted;
                
                await SaveTasksAsync(tasks);
                return existingTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var tasks = await GetAllTasksAsync();
                var task = tasks.FirstOrDefault(t => t.Id == id);
                
                if (task == null)
                {
                    return false;
                }

                tasks.Remove(task);
                await SaveTasksAsync(tasks);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return false;
            }
        }

        public async Task<bool> ToggleTaskCompletionAsync(int id)
        {
            try
            {
                var tasks = await GetAllTasksAsync();
                var task = tasks.FirstOrDefault(t => t.Id == id);
                
                if (task == null)
                {
                    return false;
                }

                task.IsCompleted = !task.IsCompleted;
                await SaveTasksAsync(tasks);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling task completion");
                return false;
            }
        }

        private async Task SaveTasksAsync(List<TaskItem> tasks)
        {
            _logger.LogInformation("Saving {Count} tasks to file", tasks.Count);
            
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            _logger.LogInformation("JSON serialized, length: {Length}", json.Length);
            
            await File.WriteAllTextAsync(_filePath, json);
            _logger.LogInformation("Tasks saved to file successfully");
        }
    }
} 
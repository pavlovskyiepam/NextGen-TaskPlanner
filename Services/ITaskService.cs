using TaskPlanner.Models;

namespace TaskPlanner.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int id);
        Task<bool> ToggleTaskCompletionAsync(int id);
    }
} 
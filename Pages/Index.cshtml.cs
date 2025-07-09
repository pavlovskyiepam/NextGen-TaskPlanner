using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using TaskPlanner.Models;
using TaskPlanner.Services;
using TaskPlanner;

namespace TaskPlanner.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<IndexModel> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public IndexModel(ITaskService taskService, ILogger<IndexModel> logger, IStringLocalizer<SharedResource> localizer)
        {
            _taskService = taskService;
            _logger = logger;
            _localizer = localizer;
        }

        public List<TaskItem> Tasks { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                Tasks = await _taskService.GetAllTasksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tasks");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingTasks"].Value;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var success = await _taskService.DeleteTaskAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = _localizer["TaskDeleted"].Value;
                }
                else
                {
                    TempData["ErrorMessage"] = _localizer["TaskNotFoundOrCannotDelete"].Value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task with ID {TaskId}", id);
                TempData["ErrorMessage"] = _localizer["ErrorDeletingTask"].Value;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleCompletionAsync(int id)
        {
            try
            {
                var success = await _taskService.ToggleTaskCompletionAsync(id);
                if (success)
                {
                    var task = await _taskService.GetTaskByIdAsync(id);
                    if (task != null)
                    {
                        TempData["SuccessMessage"] = task.IsCompleted ? _localizer["TaskCompleted"].Value : _localizer["TaskUncompleted"].Value;
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = _localizer["TaskNotFoundOrCannotUpdate"].Value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling task completion for ID {TaskId}", id);
                TempData["ErrorMessage"] = _localizer["ErrorUpdatingTask"].Value;
            }

            return RedirectToPage();
        }
    }
}

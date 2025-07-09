using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using TaskPlanner.Models;
using TaskPlanner.Services;
using TaskPlanner;

namespace TaskPlanner.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<EditModel> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public EditModel(ITaskService taskService, ILogger<EditModel> logger, IStringLocalizer<SharedResource> localizer)
        {
            _taskService = taskService;
            _logger = logger;
            _localizer = localizer;
        }

        [BindProperty]
        public TaskItem Task { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    TempData["ErrorMessage"] = _localizer["TaskNotFound"];
                    return RedirectToPage("/Index");
                }

                Task = task;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading task with ID {TaskId}", id);
                TempData["ErrorMessage"] = _localizer["ErrorLoadingTask"];
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _taskService.UpdateTaskAsync(Task);
                TempData["SuccessMessage"] = _localizer["TaskUpdated"];
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task with ID {TaskId}", Task.Id);
                ModelState.AddModelError("", _localizer["ErrorUpdatingTask"]);
                return Page();
            }
        }
    }
} 
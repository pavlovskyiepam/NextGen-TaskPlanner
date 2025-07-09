using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using TaskPlanner.Models;
using TaskPlanner.Services;
using TaskPlanner;

namespace TaskPlanner.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<CreateModel> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CreateModel(ITaskService taskService, ILogger<CreateModel> logger, IStringLocalizer<SharedResource> localizer)
        {
            _taskService = taskService;
            _logger = logger;
            _localizer = localizer;
        }

        [BindProperty]
        public TaskItem Task { get; set; } = new();

        public void OnGet()
        {
            _logger.LogInformation("Create page GET method called");
            // Initialize with default values
            Task.DueDate = DateTime.Today.AddDays(1);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Create page POST method called");
            _logger.LogInformation("Task Title: {Title}", Task.Title);
            _logger.LogInformation("Task Description: {Description}", Task.Description);
            _logger.LogInformation("Task DueDate: {DueDate}", Task.DueDate);
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {Error}", error.ErrorMessage);
                }
                return Page();
            }

            try
            {
                _logger.LogInformation("Creating task...");
                await _taskService.CreateTaskAsync(Task);
                _logger.LogInformation("Task created successfully");
                TempData["SuccessMessage"] = _localizer["TaskCreated"].Value;
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                ModelState.AddModelError("", _localizer["ErrorCreatingTask"].Value);
                return Page();
            }
        }
    }
} 
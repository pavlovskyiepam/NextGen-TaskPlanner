using System.ComponentModel.DataAnnotations;

namespace TaskPlanner.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "TitleRequired")]
        [StringLength(100, ErrorMessage = "TitleLength")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "DescriptionLength")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "DueDateRequired")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 
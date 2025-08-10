using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularApplication.Models
{
    public class ShiftDetail
    {
        public int Id { get; set; }

        [Required]
        public int ShiftId { get; set; }  // FK to Shift

        [Required]
        public string TaskDescription { get; set; } = string.Empty;

        [Required]
        public DateTime TaskStartTime { get; set; }

        [Required]
        public DateTime TaskEndTime { get; set; }

        [Required]
        public string TaskType { get; set; } = string.Empty; // e.g., "Setup", "Service", "Cleanup"

        public string? Notes { get; set; }

        public bool IsCompleted { get; set; }

        // Navigation property
        public Shift? Shift { get; set; }
    }
}

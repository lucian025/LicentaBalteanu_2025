using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicentaBalteanu.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public float? Weight { get; set; }
        public float? Height { get; set; }
        public string? Gender { get; set; } // M, F, Nu spun
        public bool? isSuffering{ get; set; }
        public ICollection<UserAnswer>? Answers { get; set; }
        public TrainingPlan? TrainingPlan { get; set; }
        public DietPlan? DietPlan { get; set; }

    }

    public class Question
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Text { get; set; } = string.Empty;

        public ICollection<UserAnswer>? Answers { get; set; }
    }

    public class UserAnswer
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        [Required]
        public bool Answer { get; set; }
    }

    public class TrainingPlan
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<PlanEntry>? Entries { get; set; }
    }

    public class DietPlan
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PlanEntry>? Entries { get; set; }
    }

    public class PlanEntry
    {
        public int Id { get; set; }

        [Required]
        public DayOfWeekCustom DayOfWeek { get; set; } = DayOfWeekCustom.Luni;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(255)]
        public string Content { get; set; } = string.Empty;

        public int? TrainingPlanId { get; set; }
        [ForeignKey("TrainingPlanId")]
        public TrainingPlan? TrainingPlan { get; set; }

        public int? DietPlanId { get; set; }
        [ForeignKey("DietPlanId")]
        public DietPlan? DietPlan { get; set; }
    }

    public enum DayOfWeekCustom
    {
        Luni = 0,
        Marti = 1,
        Miercuri = 2,
        Joi = 3,
        Vineri = 4,
        Sambata = 5,
        Duminica = 6
    }

}

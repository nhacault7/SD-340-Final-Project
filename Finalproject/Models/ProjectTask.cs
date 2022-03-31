
using Finalproject.Data;

namespace Finalproject.Models
{
    public class ProjectTask
    {
        public ProjectTask()
        {
            Comments = new HashSet<Comment>();
            Notifications = new HashSet<TaskNotification>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public double? PercentageCompleted { get; set; }
        public bool? IsCompleted { get; set; }
        public int? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public int ProjectId { get; set; }

        public virtual ApplicationUser UserCreator { get; set; }
        public virtual Project Project { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<TaskNotification> Notifications { get; set; }
    }
}


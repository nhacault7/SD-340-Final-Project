

using Finalproject.Data;

namespace Finalproject.Models
{
    public class Project
    {
        public Project()
        {
            Notifications = new HashSet<ProjectNotification>();
            Tasks = new HashSet<ProjectTask>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? PercentageCompleted { get; set; }
        public bool? IsCompleted { get; set; }
        public double? Budget { get; set; }
        public float? TotalCost { get; set; }
        public Priority? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? Deadline { get; set; }

        public virtual ICollection<UserProject> UserProjects { get; set; }

        public virtual ICollection<ProjectNotification> Notifications { get; set; }
        public virtual ICollection<ProjectTask> Tasks { get; set; }
    }

    public enum Priority
    {
        Urgent,
        High,
        Medium,
        Low
    }
}


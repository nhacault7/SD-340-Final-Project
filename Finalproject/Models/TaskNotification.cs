namespace Finalproject.Models
{
    public class TaskNotification : Notification    
    {
        public virtual ProjectTask? Task { get; set; }
    }
}

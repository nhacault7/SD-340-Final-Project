using Finalproject.Data;

namespace Finalproject.Models
{
    public abstract class Notification
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsRead { get; set; }
        public int? TaskId { get; set; }
        public int? ProjectId { get; set; }
        public string? Discriminator { get; set; }

        public virtual ApplicationUser UserCreator { get; set; }

    }
}

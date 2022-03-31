using Finalproject.Data;

namespace Finalproject.Models
{
    public class Comment
    {
      
            public int Id { get; set; }
            public string? Text { get; set; }
            public int? CommentType { get; set; }
            public int? TaskId { get; set; }

            public virtual ProjectTask? Task { get; set; }

            public virtual ApplicationUser UserCreator { get; set; }

    }
}

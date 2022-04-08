using Finalproject.Data;
using System.ComponentModel.DataAnnotations;

namespace Finalproject.Models
{
    public class Comment
    {
      
            public int Id { get; set; }
            public string? Text { get; set; }
            public CommentType? CommentType { get; set; }
            public int? TaskId { get; set; }

            public virtual ProjectTask? Task { get; set; }

            public virtual ApplicationUser UserCreator { get; set; }

    }

    public enum CommentType
    {
        [Display(Name = "Common")]
        Common,

        [Display(Name = "Urgent")]
        Urgent,

    }
}

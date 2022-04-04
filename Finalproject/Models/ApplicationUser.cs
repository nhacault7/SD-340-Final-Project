using Microsoft.AspNetCore.Identity;

namespace Finalproject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public float DailySalary { get; set; }

        public virtual ICollection<UserProject> UserProjects { get; set; }

    }
}

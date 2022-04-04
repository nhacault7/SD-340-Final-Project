using Finalproject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Finalproject.Data
{
    //clarify applicationDbContext extends the IdentityDbContext, the type of user, role and key type<TKey>
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Project> Projects { get; set; } = null!;
        public virtual DbSet<ProjectTask> Tasks { get; set; } = null!;
        public virtual DbSet<UserProject> UserProjects { get; set; } = null!;



    }
}
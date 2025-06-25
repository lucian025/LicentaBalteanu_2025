using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LicentaBalteanu.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<DietPlan> DietPlans { get; set; }
        public DbSet<PlanEntry> PlanEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TrainingPlan>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            builder.Entity<DietPlan>()
                .HasIndex(p => p.UserId)
                .IsUnique();
        }


    }


}

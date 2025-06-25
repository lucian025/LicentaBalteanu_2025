using LicentaBalteanu.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class QuestionService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public QuestionService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<Question>> GetRandomQuestionsAsync(int count = 7)
    {
        using var context = _contextFactory.CreateDbContext();
        return await context.Questions
            .OrderBy(q => Guid.NewGuid())
            .Take(count)
            .ToListAsync();
    }

    public async Task SaveUserAnswersAsync(string userId, Dictionary<int, bool> answers, int threshold = 5)
    {
        using var context = _contextFactory.CreateDbContext();
        var user = await context.Users
            .Include(u => u.DietPlan).ThenInclude(p => p.Entries)
            .Include(u => u.TrainingPlan).ThenInclude(p => p.Entries)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return;

        // Actualizează sau creează DietPlan
        var newDiet = GenerateDietPlan(user);
        if (user.DietPlan != null)
        {
            context.PlanEntries.RemoveRange(user.DietPlan.Entries);
            user.DietPlan.Description = newDiet.Description;
            user.DietPlan.Entries = newDiet.Entries;
        }
        else
        {
            user.DietPlan = newDiet;
        }

        // Actualizează sau creează TrainingPlan
        var newTraining = GenerateTrainingPlan(user);
        if (user.TrainingPlan != null)
        {
            context.PlanEntries.RemoveRange(user.TrainingPlan.Entries);
            user.TrainingPlan.Description = newTraining.Description;
            user.TrainingPlan.Entries = newTraining.Entries;
        }
        else
        {
            user.TrainingPlan = newTraining;
        }

        // Salvează răspunsurile
        var existingAnswers = await context.UserAnswers
            .Where(a => a.UserId == userId)
            .ToListAsync();
        context.UserAnswers.RemoveRange(existingAnswers);

        foreach (var answer in answers)
        {
            context.UserAnswers.Add(new UserAnswer
            {
                UserId = userId,
                QuestionId = answer.Key,
                Answer = answer.Value
            });
        }

        user.isSuffering = answers.Count(a => a.Value) >= threshold;
        await context.SaveChangesAsync();
    }

    float CalculateBMR(float weight, float height, int age, string gender)
    {
        return gender == "M"
            ? 10 * weight + 6.25f * height - 5 * age + 5
            : 10 * weight + 6.25f * height - 5 * age - 161;
    }

    DietPlan GenerateDietPlan(ApplicationUser user)
    {
        float weight = user.Weight ?? 70;
        float height = user.Height ?? 170;
        int age = user.Age ?? 25;
        string gender = user.Gender ?? "F";

        float bmr = CalculateBMR(weight, height, age, gender);
        float calFactor = bmr / 2000f;
        string description = $"Plan pe 7 zile (~{Math.Round(bmr)} kcal/zi, factor {calFactor:F2}).";

        var proteine = new[] { "piept de pui", "curcan", "somon", "ton", "linte", "năut", "ouă" };
        var garnituri = new[] { "orez brun", "cartof dulce", "quinoa", "paste integrale", "hrișcă", "bulgur", "cuscus" };
        var legume = new[] { "broccoli", "dovlecel", "ardei", "sparanghel", "morcovi", "vinete", "fasole verde" };

        var plan = new DietPlan
        {
            UserId = user.Id,
            Description = description,
            Entries = new List<PlanEntry>()
        };

        var days = Enum.GetValues<DayOfWeekCustom>();
        int i = 0;
        foreach (var day in days)
        {
            int g(float baseGrams) => (int)(Math.Round(baseGrams * calFactor / 5f) * 5);

            plan.Entries.Add(new PlanEntry
            {
                DayOfWeek = day,
                StartTime = TimeSpan.Parse("06:00"),
                EndTime = TimeSpan.Parse("08:00"),
                Content = $"Mic dejun:\n{g(80)}g - ovăz\n1 - banană\n250ml - lapte vegetal"
            });

            plan.Entries.Add(new PlanEntry
            {
                DayOfWeek = day,
                StartTime = TimeSpan.Parse("13:00"),
                EndTime = TimeSpan.Parse("14:00"),
                Content = $"Prânz:\n{g(150)}g - {proteine[i]}\n{g(100)}g - {garnituri[i]}\nsalată cu - {legume[i]}"
            });

            string cinaProtein = proteine[(i + 3) % proteine.Length];
            plan.Entries.Add(new PlanEntry
            {
                DayOfWeek = day,
                StartTime = TimeSpan.Parse("18:00"),
                EndTime = TimeSpan.Parse("19:00"),
                Content = $"Cină:\n{g(120)}g - {cinaProtein}\nlegume - {legume[(i + 2) % legume.Length]}\n1 linguriță - ulei de măsline"
            });

            i++;
        }

        return plan;
    }

    TrainingPlan GenerateTrainingPlan(ApplicationUser user)
    {
        bool heavy = user.Weight > 80 || (user.Age ?? 0) < 40;
        int sets = heavy ? 4 : 3;
        int repsHi = heavy ? 12 : 10;
        int repsLo = heavy ? 15 : 12;

        string description = "Plan pe 7 zile, 3 sesiuni/zi (mobilitate, forță, cardio).";

        var plan = new TrainingPlan
        {
            UserId = user.Id,
            Description = description,
            Entries = new List<PlanEntry>()
        };

        var days = Enum.GetValues<DayOfWeekCustom>();
        int d = 0;
        foreach (var day in days)
        {
            string focus = d switch
            {
                0 => "piept & triceps",
                1 => "spate & biceps",
                2 => "picioare",
                3 => "umeri & core",
                4 => "full-body circuit",
                5 => "HIIT + core",
                _ => "stretching activ"
            };

            plan.Entries.Add(new PlanEntry
            {
                DayOfWeek = day,
                StartTime = TimeSpan.Parse("06:30"),
                EndTime = TimeSpan.Parse("07:15"),
                Content = "Mobilitate:\n5-10 min foam-rolling\n20 min stretching dinamic\n5 min respirație diafragmatică"
            });

            plan.Entries.Add(new PlanEntry
            {
                DayOfWeek = day,
                StartTime = TimeSpan.Parse("13:00"),
                EndTime = TimeSpan.Parse("14:00"),
                Content = $"{sets}x{repsHi} - {focus}\n{sets}x{repsLo} - plank\n{sets}x{repsLo} - ridicări picioare"
            });

            plan.Entries.Add(new PlanEntry
            {
                DayOfWeek = day,
                StartTime = TimeSpan.Parse("18:00"),
                EndTime = TimeSpan.Parse("19:00"),
                Content = d switch
                {
                    5 => "HIIT:\n20 min (1 min sprint / 1 min mers)\n10 min stretching",
                    6 => "Cardio ușor:\nPlimbare 45 min\n10 min meditație",
                    _ => "Cardio moderat:\n30 min bicicletă/eliptică\n15 min stretching"
                }
            });

            d++;
        }

        return plan;
    }
}

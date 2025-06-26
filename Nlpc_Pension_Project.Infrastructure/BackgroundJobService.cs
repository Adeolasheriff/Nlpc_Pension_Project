

using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nlpc_Pension_Project.Domain.Entities;
using Nlpc_Pension_Project.Domain.Enums;

namespace Nlpc_Pension_Project.Infrastructure;

// BackgroundJobs.cs
public class BackgroundJobs : IBackgroundJobService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceProvider _serviceProvider;

    public BackgroundJobs(IServiceProvider serviceProvider, IRecurringJobManager recurringJobManager)
    {
        _serviceProvider = serviceProvider;
        _recurringJobManager = recurringJobManager;
    }

    public void RegisterJobs()
    {
        _recurringJobManager.AddOrUpdate(
            "GenerateMonthlyReports",
            () => GenerateMonthlyReports(),
            Cron.Monthly);

        _recurringJobManager.AddOrUpdate(
            "UpdateBenefitEligibility",
            () => UpdateBenefitEligibility(),
            Cron.Daily);

        _recurringJobManager.AddOrUpdate(
            "CalculateMonthlyInterest",
            () => CalculateMonthlyInterest(),
            Cron.Monthly);

        _recurringJobManager.AddOrUpdate(
            "GenerateMemberStatements",
            () => GenerateMemberStatements(),
            Cron.Weekly);
    }

    
        //  monthly validation contribution report
        public void GenerateMonthlyReports()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var report = context.Members
            .Include(m => m.Contributions)
            .Where(m => !m.IsDeleted)
            .Select(m => new
            {
                m.Id,
                m.FirstName,
                m.LastName,
                HasContributedThisMonth = m.Contributions
                    .Any(c => c.Type == ContributionType.Monthly &&
                              c.ContributionDate.Month == currentMonth &&
                              c.ContributionDate.Year == currentYear)
            })
            .Where(x => !x.HasContributedThisMonth)
            .ToList();

        // Optional: save to report table or log
        Console.WriteLine($"[MonthlyReport] Members without contribution this month: {report.Count}");
    }
    

   

        // Benefit eligibility update logic
        public void UpdateBenefitEligibility()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var allMembers = context.Members
            .Include(m => m.Contributions)
            .Include(m => m.Benefits)
            .Where(m => !m.IsDeleted)
            .ToList();

        const int requiredMonths = 60;

        foreach (var member in allMembers)
        {
            var months = member.Contributions
                .Select(c => new DateTime(c.ContributionDate.Year, c.ContributionDate.Month, 1))
                .Distinct()
                .Count();

            var eligible = months >= requiredMonths;

            var existingBenefit = member.Benefits.FirstOrDefault(b => b.Type == BenefitType.Retirement);

            if (existingBenefit != null)
            {
                existingBenefit.EligibilityStatus = eligible;
                existingBenefit.CalculationDate = DateTime.UtcNow;
            }
            else
            {
                context.Benefits.Add(new Benefit
                {
                    MemberId = member.Id,
                    Type = BenefitType.Retirement,
                    EligibilityStatus = eligible,
                    CalculationDate = DateTime.UtcNow,
                    Amount = 0 // to be calculated later
                });
            }
        }

        context.SaveChanges();
    }

    // logic to calculate monthly interest
    public void CalculateMonthlyInterest()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var voluntaryContributions = context.Contributions
            .Where(c => c.Type == ContributionType.Voluntary &&
                        c.ContributionDate.Month == DateTime.UtcNow.Month &&
                        c.ContributionDate.Year == DateTime.UtcNow.Year)
            .ToList();

        foreach (var contribution in voluntaryContributions)
        {
            var interest = contribution.Amount * 0.005m;
            contribution.Amount += interest;
        }

        context.SaveChanges();

        Console.WriteLine($"[InterestCalc] Applied interest to {voluntaryContributions.Count} voluntary contributions.");
    }

    // logic to generate member statement
    public void GenerateMemberStatements()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var members = context.Members
            .Include(m => m.Contributions)
            .Where(m => !m.IsDeleted)
            .ToList();

        foreach (var member in members)
        {
            var thisMonthContributions = member.Contributions
                .Where(c => c.ContributionDate.Month == currentMonth && c.ContributionDate.Year == currentYear)
                .ToList();

            if (!thisMonthContributions.Any()) continue;

            var total = thisMonthContributions.Sum(c => c.Amount);
            Console.WriteLine($"[Statement] Member {member.Id} - {member.FirstName}: Total this month: {total:C}");

            // Optionally: Write to file, database, or email
        }
    }
}
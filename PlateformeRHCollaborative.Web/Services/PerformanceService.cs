using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Services;

public interface IPerformanceService
{
    Task<double> CalculateScoreAsync(int evaluationId);
    Task<decimal> CalculateBonusAsync(int evaluationId);
    Task InitializeDefaultCriteriaAsync();
}

public class PerformanceService : IPerformanceService
{
    private readonly ApplicationDbContext _context;

    public PerformanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<double> CalculateScoreAsync(int evaluationId)
    {
        var evaluation = await _context.PerformanceEvaluations
            .Include(e => e.Details)
            .ThenInclude(d => d.Criteria)
            .FirstOrDefaultAsync(e => e.Id == evaluationId);

        if (evaluation == null || !evaluation.Details.Any()) return 0;

        double weightedSum = 0;
        double totalWeight = 0;

        foreach (var detail in evaluation.Details)
        {
            if (detail.Criteria != null)
            {
                // Score is 1-10, normalize or use as is? 
                // Let's say score is out of 10.
                weightedSum += detail.Score * detail.Criteria.Weight;
                totalWeight += detail.Criteria.Weight;
            }
        }

        if (totalWeight == 0) return 0;

        // Final score normalized to 100% if weight sum is 100
        // If criteria scores are 1-10, max possible is 10 * 100 = 1000
        // Formula: (WeightedSum / (MaxScore * TotalWeight)) * 100
        double maxScore = 10.0;
        double scorePercent = (weightedSum / (maxScore * totalWeight)) * 100;

        evaluation.FinalScore = scorePercent;
        await _context.SaveChangesAsync();

        return scorePercent;
    }

    public async Task<decimal> CalculateBonusAsync(int evaluationId)
    {
        var evaluation = await _context.PerformanceEvaluations
            .Include(e => e.Employee)
            .FirstOrDefaultAsync(e => e.Id == evaluationId);

        if (evaluation == null || evaluation.Employee == null) return 0;

        decimal salary = evaluation.Employee.SalaryBase;
        double score = evaluation.FinalScore;

        decimal bonusPercentage = 0;

        if (score >= 85) bonusPercentage = 0.15m;
        else if (score >= 70) bonusPercentage = 0.10m;
        else if (score >= 60) bonusPercentage = 0.05m;
        else bonusPercentage = 0m;

        evaluation.CalculatedBonus = salary * bonusPercentage;
        await _context.SaveChangesAsync();

        return evaluation.CalculatedBonus;
    }

    public async Task InitializeDefaultCriteriaAsync()
    {
        if (!await _context.EvaluationCriteria.AnyAsync())
        {
            _context.EvaluationCriteria.AddRange(new List<EvaluationCriteria>
            {
                new EvaluationCriteria { Name = "Performance (objectifs atteints)", Weight = 40, Description = "Mesure l'atteinte des objectifs fixés" },
                new EvaluationCriteria { Name = "Ponctualité", Weight = 20, Description = "Respect des horaires et des délais" },
                new EvaluationCriteria { Name = "Qualité du travail", Weight = 20, Description = "Précision et fiabilité des livrables" },
                new EvaluationCriteria { Name = "Travail d'équipe", Weight = 10, Description = "Collaboration avec les collègues" },
                new EvaluationCriteria { Name = "Communication", Weight = 10, Description = "Clarté et efficacité des échanges" }
            });
            await _context.SaveChangesAsync();
        }
    }
}

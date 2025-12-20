using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlateformeRHCollaborative.Web.Models;

public class EvaluationDetail
{
    public int Id { get; set; }

    [Required]
    public int PerformanceEvaluationId { get; set; }
    [ForeignKey("PerformanceEvaluationId")]
    public virtual PerformanceEvaluation? PerformanceEvaluation { get; set; }

    [Required]
    public int EvaluationCriteriaId { get; set; }
    [ForeignKey("EvaluationCriteriaId")]
    public virtual EvaluationCriteria? Criteria { get; set; }

    [Range(1, 10)]
    public int Score { get; set; } // Score on a scale (e.g., 1-10)
}

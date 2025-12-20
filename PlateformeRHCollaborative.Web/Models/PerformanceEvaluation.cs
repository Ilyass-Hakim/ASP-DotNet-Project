using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlateformeRHCollaborative.Web.Models;

public enum EvaluationStatus
{
    Draft,
    Submitted,
    Validated
}

public enum BonusStatus
{
    Pending,
    Approved,
    Rejected
}

public class PerformanceEvaluation
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    public virtual Employee? Employee { get; set; }

    [Required]
    public string ManagerId { get; set; } = string.Empty; // Identity UserId of the manager

    public DateTime EvaluationDate { get; set; } = DateTime.Now;

    public string Period { get; set; } = string.Empty; // e.g. "2025"

    public double FinalScore { get; set; } // Normalized score (e.g., 0-100)
    
    public EvaluationStatus Status { get; set; } = EvaluationStatus.Draft;

    public decimal CalculatedBonus { get; set; }
    public BonusStatus BonusStatus { get; set; } = BonusStatus.Pending;
    
    public string? RHComments { get; set; }

    public virtual ICollection<EvaluationDetail> Details { get; set; } = new List<EvaluationDetail>();
}

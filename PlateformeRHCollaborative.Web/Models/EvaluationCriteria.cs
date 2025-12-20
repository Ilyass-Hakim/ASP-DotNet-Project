using System.ComponentModel.DataAnnotations;

namespace PlateformeRHCollaborative.Web.Models;

public class EvaluationCriteria
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100)]
    public double Weight { get; set; } // Percentage

    public string Description { get; set; } = string.Empty;
}

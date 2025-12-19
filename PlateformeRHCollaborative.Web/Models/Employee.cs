namespace PlateformeRHCollaborative.Web.Models;

public class Employee
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Poste { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int SoldeConges { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal SalaryBase { get; set; } = 3000m;
    public int PerformancePoints { get; set; } = 0;
}






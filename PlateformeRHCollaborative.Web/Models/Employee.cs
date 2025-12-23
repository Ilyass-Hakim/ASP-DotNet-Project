namespace PlateformeRHCollaborative.Web.Models;

public class Employee
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty; // Legacy Display Name or Full Name
    
    // Detailed Profile Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = "/images/default-avatar.png";
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }

    // Professional Info
    public string Matricule { get; set; } = string.Empty; // Employee ID
    public string Poste { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int SoldeConges { get; set; }
    public string Role { get; set; } = string.Empty;
    public decimal SalaryBase { get; set; } = 3000m;
    public DateTime HireDate { get; set; } = DateTime.Today;
    public string ContractType { get; set; } = "CDI";
    public bool IsActive { get; set; } = true;

    // New Organizational Fields
    public string Department { get; set; } = "Non assigné";
    public int? ManagerId { get; set; }
    public virtual Employee? Manager { get; set; }
}






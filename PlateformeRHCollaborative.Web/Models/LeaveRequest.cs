using System;

namespace PlateformeRHCollaborative.Web.Models;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string TypeConge { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Statut { get; set; } = "EnAttente";
    public DateTime DateDemande { get; set; } = DateTime.UtcNow;
}






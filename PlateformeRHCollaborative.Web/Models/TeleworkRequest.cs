using System;

namespace PlateformeRHCollaborative.Web.Models;

public class TeleworkRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Statut { get; set; } = "EnAttente";
    public string? Motif { get; set; }
}






using System;

namespace PlateformeRHCollaborative.Web.Models;

public class DocumentRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";

    public Employee? Employee { get; set; }
}



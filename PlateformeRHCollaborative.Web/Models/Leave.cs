using System;

namespace PlateformeRHCollaborative.Web.Models;

public class Leave
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Employee? Employee { get; set; }
<<<<<<< HEAD

    // Audit fields
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedById { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectedById { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledById { get; set; }
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
}






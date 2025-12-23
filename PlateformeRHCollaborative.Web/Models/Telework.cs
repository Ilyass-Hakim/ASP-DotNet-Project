using System;

namespace PlateformeRHCollaborative.Web.Models;

public class Telework
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Motif { get; set; }
    public string Status { get; set; } = "Pending";
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Employee? Employee { get; set; }

    // Audit fields
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedById { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectedById { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelledById { get; set; }
}






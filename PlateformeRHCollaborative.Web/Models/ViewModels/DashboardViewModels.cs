using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Models.ViewModels;

public class EmployeeDashboardVM
{
    public Employee Employee { get; set; } = new();
    public int PendingRequestsCount { get; set; }
    public int DaysTakenThisYear { get; set; }
    public int DaysPlanned { get; set; }
    public List<CombinedRequestVM> RecentRequests { get; set; } = new();
    public List<TeamMemberAbsenceVM> AbsentColleagues { get; set; } = new();
}

public class ManagerDashboardVM
{
    public Employee Manager { get; set; } = new();
    public int TeamSize { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int PendingRequestsToValidate { get; set; }
    public List<TeamPlanningDayVM> WeeklyPlanning { get; set; } = new();
    public List<EmployeeBalanceVM> TeamBalances { get; set; } = new();
}

public class RHDashboardVM
{
    public Employee CurrentUser { get; set; } = new();
    public int TotalEmployees { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalTeleworking { get; set; }
    public double LeaveUtilizationRate { get; set; }
    public int TotalPendingRequests { get; set; }
    public List<DepartmentStatsVM> DepartmentStats { get; set; } = new();
    public List<CombinedRequestVM> RecentGlobalRequests { get; set; } = new();
    public List<string> LowBalanceAlerts { get; set; } = new();
}

public class DirectorDashboardVM
{
    public Employee CurrentUser { get; set; } = new();
    
    // KPI Data
    public int TotalEmployees { get; set; }
    public double PresenceRate { get; set; }
    public double PresenceRateTrend { get; set; } // +2.5 or -1.0
    public int TotalManagers { get; set; }
    public int TotalPendingRequests { get; set; }

    // Chart Data
    public List<DepartmentStatsVM> DepartmentPerformance { get; set; } = new();
    public List<RequestStatusStatsVM> RequestStatusDistribution { get; set; } = new(); // For Donut Chart
    public List<MonthlyTrendVM> PresenceTrendData { get; set; } = new(); // For Line Chart

    // Table Data
    public List<ManagerSummaryVM> ManagerSummaries { get; set; } = new();
    
    // Alerts
    public List<string> StrategicAlerts { get; set; } = new();
}

// Helper VMs for Director
public class RequestStatusStatsVM
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class MonthlyTrendVM
{
    public string Month { get; set; } = string.Empty;
    public double Rate { get; set; }
}

public class ManagerSummaryVM
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int SubordinateCount { get; set; }
    public double TeamPresenceRate { get; set; }
    public int PendingRequestsCount { get; set; }
    public bool RequiresAttention { get; set; }
}

// Shared Helper VM
public class CombinedRequestVM
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // "Congé" or "Télétravail"
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class TeamMemberAbsenceVM
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class TeamPlanningDayVM
{
    public DateTime Date { get; set; }
    public List<string> AbsentNames { get; set; } = new();
    public List<string> TeleworkingNames { get; set; } = new();
    public int PresentCount { get; set; }
}

public class EmployeeBalanceVM
{
    public string Name { get; set; } = string.Empty;
    public string Poste { get; set; } = string.Empty;
    public int Solde { get; set; }
}

public class DepartmentStatsVM
{
    public string Name { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public int AbsentCount { get; set; }
    public double AbsenceRate { get; set; }
}

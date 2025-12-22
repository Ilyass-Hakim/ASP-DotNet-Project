using System;
using System.Collections.Generic;

namespace PlateformeRHCollaborative.Web.Models.ViewModels
{
    public class LeaveBalanceViewModel
    {
        public int AnnualTotal { get; set; } = 25;
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
        public double UsagePercentage { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        
        public List<LeaveHistoryItem> History { get; set; } = new List<LeaveHistoryItem>();
    }

    public class LeaveHistoryItem
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedByName { get; set; } = "Système";
    }
}

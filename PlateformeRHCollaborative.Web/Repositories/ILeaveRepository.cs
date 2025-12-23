namespace PlateformeRHCollaborative.Web.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using PlateformeRHCollaborative.Web.Models;

public interface ILeaveRepository
{
	Task<IEnumerable<Leave>> GetAllAsync();
	Task<Leave?> GetByIdAsync(int id);
	Task AddAsync(Leave entity);
	Task UpdateAsync(Leave entity);
	Task DeleteAsync(int id);

    Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end);
    Task<int> GetApprovedCountForTeamAsync(int managerId, DateTime start, DateTime end);
    Task<IEnumerable<Leave>> GetApprovedLeavesByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<Leave>> GetLeavesByStatusAsync(string status);
    Task<int> GetPendingCountAsync(int? managerId = null); // if managerId null, global count
    Task<int> GetApprovedCountAsync();
    Task<int> GetTotalDaysTakenAsync();
    Task<IEnumerable<Leave>> GetRecentRequestsByEmployeeIdAsync(int employeeId, int count);
    Task<IEnumerable<Leave>> GetRecentRequestsGlobalAsync(int count);
    Task<IEnumerable<Leave>> GetPendingLeavesByManagerIdAsync(int managerId);
    Task<IEnumerable<Leave>> GetApprovedLeavesForDateAsync(DateTime date);
}




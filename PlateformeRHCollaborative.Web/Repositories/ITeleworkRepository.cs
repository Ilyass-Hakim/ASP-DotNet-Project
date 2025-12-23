namespace PlateformeRHCollaborative.Web.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using PlateformeRHCollaborative.Web.Models;

public interface ITeleworkRepository
{
	Task<IEnumerable<Telework>> GetAllAsync();
	Task<Telework?> GetByIdAsync(int id);
	Task AddAsync(Telework entity);
	Task UpdateAsync(Telework entity);
	Task DeleteAsync(int id);

    Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end);
    Task<int> GetApprovedCountForTeamAsync(int managerId, DateTime start, DateTime end);
    Task<IEnumerable<Telework>> GetRecentRequestsByEmployeeIdAsync(int employeeId, int count);
    Task<IEnumerable<Telework>> GetRecentRequestsGlobalAsync(int count);
    Task<IEnumerable<Telework>> GetPendingTeleworksByManagerIdAsync(int managerId);
    Task<IEnumerable<Telework>> GetApprovedTeleworksForDateAsync(DateTime date);
    Task<int> GetTotalTeleworkingCountAsync(DateTime date);
}




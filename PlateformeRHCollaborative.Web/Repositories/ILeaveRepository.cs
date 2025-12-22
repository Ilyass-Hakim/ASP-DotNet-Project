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
<<<<<<< HEAD

    Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end);
    Task<int> GetApprovedCountForTeamAsync(int managerId, DateTime start, DateTime end);
    Task<IEnumerable<Leave>> GetApprovedLeavesByEmployeeIdAsync(int employeeId);
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
}




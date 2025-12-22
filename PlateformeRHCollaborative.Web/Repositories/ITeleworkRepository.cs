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
<<<<<<< HEAD

    Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end);
    Task<int> GetApprovedCountForTeamAsync(int managerId, DateTime start, DateTime end);
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
}




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
}




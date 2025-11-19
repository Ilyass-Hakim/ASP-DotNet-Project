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
}




using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Repositories;

namespace PlateformeRHCollaborative.Web.Services;

public class TeleworkService
{
<<<<<<< HEAD
    private readonly ITeleworkRepository _repo;
    private readonly ILeaveRepository _leaveRepo;
    private readonly IEmployeeRepository _employeeRepo;

    public TeleworkService(ITeleworkRepository repo, ILeaveRepository leaveRepo, IEmployeeRepository employeeRepo)
    {
        _repo = repo;
        _leaveRepo = leaveRepo;
        _employeeRepo = employeeRepo;
    }

    public Task<IEnumerable<Telework>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Telework?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task UpdateAsync(Telework entity) => _repo.UpdateAsync(entity);

    public async Task AddAsync(Telework entity)
    {
        // 1. Chevauchement (Leave & Telework)
        if (await _repo.HasOverlapAsync(entity.EmployeeId, entity.StartDate, entity.EndDate) ||
            await _leaveRepo.HasOverlapAsync(entity.EmployeeId, entity.StartDate, entity.EndDate))
        {
            throw new BusinessException("Une autre demande (Congé ou Télétravail) existe déjà sur cette période.");
        }

        // 2. Règle des 30% (Si manager)
        var employee = await _employeeRepo.GetByIdAsync(entity.EmployeeId);
        if (employee != null && employee.ManagerId.HasValue)
        {
            await CheckTeamLimitAsync(employee.ManagerId.Value, entity.StartDate, entity.EndDate);
        }

        entity.Status = "Pending";
        await _repo.AddAsync(entity);
    }

    public async Task ApproveAsync(int id, int actorId)
    {
        var telework = await _repo.GetByIdAsync(id);
        if (telework == null) throw new BusinessException("Demande introuvable.");
        
        var actor = await _employeeRepo.GetByIdAsync(actorId);
        AuthorizeAction(telework, actor);

        // Re-check 30%
        if (telework.Employee?.ManagerId != null)
        {
            await CheckTeamLimitAsync(telework.Employee.ManagerId.Value, telework.StartDate, telework.EndDate);
        }

        telework.Status = "Approved";
        telework.ApprovedAt = DateTime.Now;
        telework.ApprovedById = actor?.UserId;

        await _repo.UpdateAsync(telework);
    }

    public async Task RejectAsync(int id, int actorId, string reason)
    {
        var telework = await _repo.GetByIdAsync(id);
        if (telework == null) throw new BusinessException("Demande introuvable.");

        var actor = await _employeeRepo.GetByIdAsync(actorId);
        AuthorizeAction(telework, actor);

        if (string.IsNullOrWhiteSpace(reason)) throw new BusinessException("Le motif est obligatoire.");

        telework.Status = "Rejected";
        telework.RejectionReason = reason;
        telework.RejectedAt = DateTime.Now;
        telework.RejectedById = actor?.UserId;

        await _repo.UpdateAsync(telework);
    }

    public async Task CancelAsync(int id, int actorId)
    {
        var telework = await _repo.GetByIdAsync(id);
        if (telework == null) throw new BusinessException("Demande introuvable.");

        if (telework.EmployeeId != actorId) throw new BusinessException("Vous ne pouvez annuler que vos propres demandes.");

        if (telework.StartDate <= DateTime.Now) throw new BusinessException("Impossible d'annuler une demande passée ou en cours.");

        telework.Status = "Cancelled";
        telework.CancelledAt = DateTime.Now;
        telework.CancelledById = (await _employeeRepo.GetByIdAsync(actorId))?.UserId;

        await _repo.UpdateAsync(telework);
    }
    
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    private void AuthorizeAction(Telework telework, Employee? actor)
    {
        if (actor == null) throw new BusinessException("Accès refusé.");
        if (telework.Employee == null) throw new BusinessException("Employé lié à la demande introuvable.");

        bool isAuthorized = false;

        if (actor.Role == "RH") isAuthorized = true;
        else if (actor.Role == "Directeur" && (telework.Employee.Role == "Manager" || telework.Employee.Role == "RH")) isAuthorized = true;
        else if (actor.Role == "Manager" && telework.Employee.ManagerId == actor.Id) isAuthorized = true;

        if (!isAuthorized) throw new BusinessException($"Vous n'avez les droits pour traiter cette demande (Rôle: {actor.Role}).");
    }

    private async Task CheckTeamLimitAsync(int managerId, DateTime start, DateTime end)
    {
        int teamSize = await _employeeRepo.GetTeamSizeAsync(managerId);
        if (teamSize == 0) return;

        int leaveCount = await _leaveRepo.GetApprovedCountForTeamAsync(managerId, start, end);
        int teleworkCount = await _repo.GetApprovedCountForTeamAsync(managerId, start, end);
        
        // RELAXED RULE: Always allow at least 1 person to be absent
        int totalAbsent = leaveCount + teleworkCount + 1;
        double ratio = (double)totalAbsent / teamSize;

        if (totalAbsent > 1 && ratio > 0.30)
            throw new BusinessException($"La limite de 30% d'absence de l'équipe serait dépassée ({ratio*100:N0}%).");
    }
=======
	private readonly ITeleworkRepository _repo;

	public TeleworkService(ITeleworkRepository repo)
	{
		_repo = repo;
	}

	public Task<IEnumerable<Telework>> GetAllAsync() => _repo.GetAllAsync();
	public Task<Telework?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
	public Task AddAsync(Telework entity) => _repo.AddAsync(entity);
	public Task UpdateAsync(Telework entity) => _repo.UpdateAsync(entity);
	public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
}




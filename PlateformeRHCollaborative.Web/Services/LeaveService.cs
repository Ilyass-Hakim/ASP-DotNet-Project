using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Repositories;

namespace PlateformeRHCollaborative.Web.Services;

public class LeaveService
{
    private readonly ILeaveRepository _repo;
    private readonly ITeleworkRepository _teleworkRepo;
    private readonly IEmployeeRepository _employeeRepo;

    public LeaveService(ILeaveRepository repo, ITeleworkRepository teleworkRepo, IEmployeeRepository employeeRepo)
    {
        _repo = repo;
        _teleworkRepo = teleworkRepo;
        _employeeRepo = employeeRepo;
    }

    public Task<IEnumerable<Leave>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Leave?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task UpdateAsync(Leave entity) => _repo.UpdateAsync(entity);

    public async Task AddAsync(Leave entity)
    {
        // 1. Délai de 48h
        if (entity.StartDate < DateTime.Now.AddHours(48))
            throw new BusinessException("La demande doit être soumise au moins 48h à l'avance.");

        // 2. Vérification du solde
        var employee = await _employeeRepo.GetByIdAsync(entity.EmployeeId);
        if (employee == null) throw new BusinessException("Employé introuvable.");

        int duration = GetBusinessDays(entity.StartDate, entity.EndDate);

        Console.WriteLine($"DEBUG - Employee ID: {employee.Id}");
        Console.WriteLine($"DEBUG - Employee Nom: {employee.Nom}");
        Console.WriteLine($"DEBUG - SoldeConges: {employee.SoldeConges}");
        Console.WriteLine($"DEBUG - Jours demandés (Ouvrables): {duration}");
        Console.WriteLine($"DEBUG - StartDate: {entity.StartDate}");
        Console.WriteLine($"DEBUG - EndDate: {entity.EndDate}");

        if (employee.SoldeConges < duration)
            throw new BusinessException($"Solde insuffisant. Requis: {duration}, Disponible: {employee.SoldeConges}");

        // 3. Chevauchement (Leave & Telework)
        if (await _repo.HasOverlapAsync(entity.EmployeeId, entity.StartDate, entity.EndDate) ||
            await _teleworkRepo.HasOverlapAsync(entity.EmployeeId, entity.StartDate, entity.EndDate))
        {
            throw new BusinessException("Une autre demande (Congé ou Télétravail) existe déjà sur cette période.");
        }

        // 4. Règle des 30% (Si manager)
        if (employee.ManagerId.HasValue)
        {
            await CheckTeamLimitAsync(employee.ManagerId.Value, entity.StartDate, entity.EndDate);
        }

        // 5. Auto-approbation (Si Directeur)
        if (employee.Role == "Directeur")
        {
            entity.Status = "Approved";
            entity.ApprovedAt = DateTime.Now;
            entity.ApprovedById = employee.UserId;
            
            // Déduction immédiate du solde
            employee.SoldeConges -= duration;
            await _employeeRepo.UpdateAsync(employee);
        }
        else
        {
            entity.Status = "Pending";
        }

        await _repo.AddAsync(entity);
    }

    public async Task ApproveAsync(int id, int actorId)
    {
        var leave = await _repo.GetByIdAsync(id);
        if (leave == null) throw new BusinessException("Demande introuvable.");
        
        var actor = await _employeeRepo.GetByIdAsync(actorId);
        if (actor == null) throw new BusinessException("Approbateur introuvable.");

        // Sécurité
        AuthorizeAction(leave, actor);

        // Re-check 30%
        if (leave.Employee?.ManagerId != null)
        {
            await CheckTeamLimitAsync(leave.Employee.ManagerId.Value, leave.StartDate, leave.EndDate);
        }

        // Déduction solde - DOUBLE VÉRIFICATION
        // On recalcule la durée en jours ouvrables
        int duration = GetBusinessDays(leave.StartDate, leave.EndDate);
        var employee = await _employeeRepo.GetByIdAsync(leave.EmployeeId);
        
        if (employee != null) 
        {
            // Vérification stricte au moment de l'approbation
            if (employee.SoldeConges < duration)
            {
                 throw new BusinessException($"Impossible d'approuver : l'employé n'a plus que {employee.SoldeConges} jours disponibles, cette demande nécessite {duration} jours.");
            }

            // Déduction effective
            employee.SoldeConges -= duration;
        }
        else
        {
             throw new BusinessException("Employé introuvable pour la déduction du solde.");
        }

        leave.Status = "Approved";
        leave.ApprovedAt = DateTime.Now;
        leave.ApprovedById = actor.UserId;

        // UpdateAsync sauvera à la fois le Leave et l'Employee (si context partagé/tracké)
        await _repo.UpdateAsync(leave);
    }

    public async Task RejectAsync(int id, int actorId, string reason)
    {
        var leave = await _repo.GetByIdAsync(id);
        if (leave == null) throw new BusinessException("Demande introuvable.");

        var actor = await _employeeRepo.GetByIdAsync(actorId);
        AuthorizeAction(leave, actor);

        if (string.IsNullOrWhiteSpace(reason)) throw new BusinessException("Le motif est obligatoire.");

        leave.Status = "Rejected";
        leave.RejectionReason = reason;
        leave.RejectedAt = DateTime.Now;
        leave.RejectedById = actor?.UserId;

        // Pas de changement de solde sur un rejet
        await _repo.UpdateAsync(leave);
    }

    public async Task CancelAsync(int id, int actorId)
    {
        var leave = await _repo.GetByIdAsync(id);
        if (leave == null) throw new BusinessException("Demande introuvable.");

        if (leave.EmployeeId != actorId) throw new BusinessException("Vous ne pouvez annuler que vos propres demandes.");

        if (leave.StartDate <= DateTime.Now) throw new BusinessException("Impossible d'annuler un congé passé ou en cours.");

        if (leave.Status == "Approved")
        {
             // Restaurer le solde
            int duration = GetBusinessDays(leave.StartDate, leave.EndDate);
            var employee = await _employeeRepo.GetByIdAsync(leave.EmployeeId);
            if (employee != null) 
            {
                employee.SoldeConges += duration;
            }
        }

        leave.Status = "Cancelled";
        leave.CancelledAt = DateTime.Now;
        leave.CancelledById = (await _employeeRepo.GetByIdAsync(actorId))?.UserId;

        await _repo.UpdateAsync(leave);
    }
    
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

    private void AuthorizeAction(Leave leave, Employee? actor)
    {
        if (actor == null) throw new BusinessException("Accès refusé.");
        if (leave.Employee == null) throw new BusinessException("Employé lié à la demande introuvable.");

        bool isAuthorized = false;

        if (actor.Role == "RH") isAuthorized = true;
        else if (actor.Role == "Directeur" && (leave.Employee.Role == "Manager" || leave.Employee.Role == "RH")) isAuthorized = true;
        else if (actor.Role == "Manager" && leave.Employee.ManagerId == actor.Id) isAuthorized = true;

        if (!isAuthorized) throw new BusinessException($"Vous n'avez les droits pour traiter cette demande (Rôle: {actor.Role}).");
    }

    private async Task CheckTeamLimitAsync(int managerId, DateTime start, DateTime end)
    {
        int teamSize = await _employeeRepo.GetTeamSizeAsync(managerId);
        if (teamSize == 0) return;

        int leaveCount = await _repo.GetApprovedCountForTeamAsync(managerId, start, end);
        int teleworkCount = await _teleworkRepo.GetApprovedCountForTeamAsync(managerId, start, end);
        
        // RELAXED RULE: Always allow at least 1 person to be absent
        int totalAbsent = leaveCount + teleworkCount + 1;
        double ratio = (double)totalAbsent / teamSize;

        if (totalAbsent > 1 && ratio > 0.30)
            throw new BusinessException($"La limite de 30% d'absence de l'équipe serait dépassée ({ratio*100:N0}%).");
    }

    public int GetBusinessDays(DateTime start, DateTime end)
    {
        int days = 0;
        for (DateTime date = start; date <= end; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                days++;
            }
        }
        return days;
    }
}

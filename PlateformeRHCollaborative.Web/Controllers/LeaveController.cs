using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Hubs;
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Services;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class LeaveController : Controller
{
    private readonly LeaveService _service;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly UserManager<IdentityUser> _userManager;
<<<<<<< HEAD
		private readonly ApplicationDbContext _context;
=======
    private readonly ApplicationDbContext _context;
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d

    public LeaveController(LeaveService service, IHubContext<NotificationsHub> hubContext, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _service = service;
        _hubContext = hubContext;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _service.GetAllAsync();
        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Leave model)
    {
<<<<<<< HEAD
        // Remove fields set by the system
        ModelState.Remove("EmployeeId");
        ModelState.Remove("Employee");
        ModelState.Remove("Status"); // Set in service
        ModelState.Remove("CreatedAt");

        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("[Create Leave] Validation Failed: " + string.Join(", ", errors));
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error); // Ensure errors are visible in summary if they aren't bound to specific fields
            }
            return View(model);
        }

        try 
        {
            // Récupérer l'utilisateur connecté
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.ErrorMessage = "Utilisateur non authentifié.";
                return View(model);
            }

            // Trouver l'Employee correspondant au UserId
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null)
            {
                ViewBag.ErrorMessage = "Aucun employé trouvé pour votre compte. Veuillez contacter les RH.";
                return View(model);
            }

            model.EmployeeId = employee.Id;
            await _service.AddAsync(model);

            // Notification temps réel aux managers
            // TODO: Targeted notification
            
            TempData["SuccessMessage"] = "Demande de congé envoyée avec succès.";
            return RedirectToAction(nameof(Index));
        }
        catch (BusinessException ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View(model);
        }
=======
        if (!ModelState.IsValid) return View(model);

        // Récupérer l'utilisateur connecté
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            ViewBag.ErrorMessage = "Utilisateur non authentifié.";
            return View(model);
        }

        // Trouver l'Employee correspondant au UserId
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        if (employee == null)
        {
            ViewBag.ErrorMessage = "Aucun employé trouvé pour votre compte. Veuillez contacter les RH.";
            return View(model);
        }

        // Assigner l'EmployeeId avant l'insertion
        model.EmployeeId = employee.Id;

        await _service.AddAsync(model);

        // Notification temps réel aux managers
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
            $"Nouvelle demande de congé du {model.StartDate:dd/MM/yyyy} au {model.EndDate:dd/MM/yyyy}.");

        return RedirectToAction(nameof(Index));
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Leave model)
    {
<<<<<<< HEAD
        // Simple update, validation rules might be bypassed here if we don't use Service.Update logic with rules.
        // But for now, user asked for Add/Approve/Reject/Cancel.
        // Edit usually restarts the process or is limited.
        // Given the scope, I'll leave Edit as basic Update but maybe add checks? 
        // Or better, disable Edit for Approved/Rejected requests?
        // Service.UpdateAsync just calls Repo.
        // Let's leave it as is for now or better: block edit if not Pending.
        
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.Status != "Pending") 
        {
             ViewBag.ErrorMessage = "Impossible de modifier une demande traitée.";
             return View(model);
        }

=======
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
        await _service.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
<<<<<<< HEAD
        try
        {
            var userId = _userManager.GetUserId(User);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            
            if (employee != null)
            {
                await _service.CancelAsync(id, employee.Id);
                TempData["SuccessMessage"] = "Demande annulée avec succès.";
            }
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Solde()
    {
        // 1. Récupérer l'employé connecté
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

        // Utilisation explicite du DbSet Employees pour le premier accès si le Repo dédié n'est pas injecté ici
        // Mais nous avons _context.
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        
        if (employee == null)
        {
             TempData["ErrorMessage"] = "Employé introuvable.";
             return RedirectToAction("Index", "Home");
        }

        // 2. Initialiser le ViewModel
        var model = new PlateformeRHCollaborative.Web.Models.ViewModels.LeaveBalanceViewModel
        {
            AnnualTotal = 25, // Fixe selon la règle
            EmployeeName = employee.Nom,
            RemainingDays = employee.SoldeConges
        };

        // 3. Récupérer l'historique des congés approuvés pour calculer le "Déjà pris"
        // Note: Nous n'avons pas accès direct au LeaveRepository via l'interface standard ici (car injecté via Service)
        // Nous allons passer par le service ou étendre le service.
        // Option propre: Ajouter GetApprovedLeavesForEmployee dans LeaveService qui appelle le Repo.
        // Option rapide ici (puisqu'on a accès au DbContext via _context si besoin, ou on cast le service/repo) :
        // Le mieux est de rester cohérent et d'ajouter la méthode proxy dans le service.
        
        // Comme je ne peux pas modifier le Service ET le Controller en même temps dans ce tool call,
        // je vais implémenter la logique ici en attendant, ou utiliser le DbSet directement pour l'historique
        // afin de garantir que ça marche tout de suite sans modifier ILeaveService.
        
        // Approche directe via _service.GetAllAsync déjà existant et filtrage (moins performant mais sûr)
        // OU mieux: Utiliser une nouvelle méthode dans le Controller qui tape dans _context pour l'historique optimisé.
        
        var approvedLeaves = await _context.Leaves
            .Where(l => l.EmployeeId == employee.Id && l.Status == "Approved")
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();

        int totalUsed = 0;
        foreach (var leave in approvedLeaves)
        {
            // Calcul précis des jours ouvrables
            int duration = _service.GetBusinessDays(leave.StartDate, leave.EndDate);
            totalUsed += duration;

            // Récupérer le nom de l'approbateur
            string approverName = "Système";
            if (!string.IsNullOrEmpty(leave.ApprovedById))
            {
                var approver = await _context.Users.FindAsync(leave.ApprovedById); // IdentityUser
                // Ou mieux chercher dans Employee
                var approverEmp = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == leave.ApprovedById);
                if (approverEmp != null) approverName = approverEmp.Nom;
                else if (approver != null) approverName = approver.UserName ?? "Inconnu";
            }

            model.History.Add(new PlateformeRHCollaborative.Web.Models.ViewModels.LeaveHistoryItem
            {
                StartDate = leave.StartDate,
                EndDate = leave.EndDate,
                Duration = duration,
                Status = leave.Status,
                ApprovedAt = leave.ApprovedAt,
                ApprovedByName = approverName
            });
        }

        model.UsedDays = totalUsed;
        
        // Calcul du pourcentage
        if (model.AnnualTotal > 0)
        {
            model.UsagePercentage = Math.Round(((double)model.UsedDays / model.AnnualTotal) * 100, 1);
        }

        return View(model);
=======
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Solde()
    {
        return View();
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    }

    public async Task<IActionResult> MyLeaves()
    {
<<<<<<< HEAD
		// Filtrer les congés pour l'employé actuellement connecté
		var userId = _userManager.GetUserId(User);
		if (string.IsNullOrEmpty(userId))
		{
			ViewBag.ErrorMessage = "Utilisateur non authentifié.";
			return View("List", Enumerable.Empty<Leave>());
		}

		var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
		if (employee == null)
		{
			ViewBag.ErrorMessage = "Aucun employé trouvé pour votre compte. Veuillez contacter les RH.";
			return View("List", Enumerable.Empty<Leave>());
		}

		var items = await _service.GetAllAsync();
		var myItems = items.Where(l => l.EmployeeId == employee.Id).ToList();
		return View("List", myItems);
=======
        var items = await _service.GetAllAsync();
        return View("List", items);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    }

    public async Task<IActionResult> History()
    {
<<<<<<< HEAD
		// Historique limité à l'employé actuellement connecté
		var userId = _userManager.GetUserId(User);
		if (string.IsNullOrEmpty(userId))
		{
			ViewBag.ErrorMessage = "Utilisateur non authentifié.";
			return View("History", Enumerable.Empty<Leave>());
		}

		var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
		if (employee == null)
		{
			ViewBag.ErrorMessage = "Aucun employé trouvé pour votre compte. Veuillez contacter les RH.";
			return View("History", Enumerable.Empty<Leave>());
		}

		var items = await _service.GetAllAsync();
		var myItems = items.Where(l => l.EmployeeId == employee.Id).ToList();
		return View("History", myItems);
=======
        var items = await _service.GetAllAsync();
        return View("History", items);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    }
}



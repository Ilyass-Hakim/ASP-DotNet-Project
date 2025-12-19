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
		private readonly ApplicationDbContext _context;

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
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Solde()
    {
        return View();
    }

    public async Task<IActionResult> MyLeaves()
    {
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
    }

    public async Task<IActionResult> History()
    {
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
    }
}



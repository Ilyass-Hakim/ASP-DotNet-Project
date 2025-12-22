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
public class TeleworkController : Controller
{
    private readonly TeleworkService _service;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly UserManager<IdentityUser> _userManager;
<<<<<<< HEAD
		private readonly ApplicationDbContext _context;
=======
    private readonly ApplicationDbContext _context;
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d

    public TeleworkController(TeleworkService service, IHubContext<NotificationsHub> hubContext, UserManager<IdentityUser> userManager, ApplicationDbContext context)
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
    public async Task<IActionResult> Create(Telework model)
    {
<<<<<<< HEAD
        // Remove fields set by the system
        ModelState.Remove("EmployeeId");
        ModelState.Remove("Employee");
        ModelState.Remove("Status");
        ModelState.Remove("CreatedAt");

        if (!ModelState.IsValid) 
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("[Create Telework] Validation Failed: " + string.Join(", ", errors));
            foreach (var error in errors)
            {
                 ModelState.AddModelError("", error);
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

            TempData["SuccessMessage"] = "Demande de télétravail envoyée avec succès.";
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
            $"Nouvelle demande de télétravail du {model.StartDate:dd/MM/yyyy} au {model.EndDate:dd/MM/yyyy}.");

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
    public async Task<IActionResult> Edit(int id, Telework model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
<<<<<<< HEAD
        
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.Status != "Pending")
        {
             ViewBag.ErrorMessage = "Impossible de modifier une demande traitée.";
             return View(model);
        }

=======
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
=======
        await _service.DeleteAsync(id);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> MyTelework()
    {
<<<<<<< HEAD
		// Filtrer le télétravail pour l'employé actuellement connecté
		var userId = _userManager.GetUserId(User);
		if (string.IsNullOrEmpty(userId))
		{
			ViewBag.ErrorMessage = "Utilisateur non authentifié.";
			return View("List", Enumerable.Empty<Telework>());
		}

		var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
		if (employee == null)
		{
			ViewBag.ErrorMessage = "Aucun employé trouvé pour votre compte. Veuillez contacter les RH.";
			return View("List", Enumerable.Empty<Telework>());
		}

		var items = await _service.GetAllAsync();
		var myItems = items.Where(t => t.EmployeeId == employee.Id).ToList();
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
			return View("History", Enumerable.Empty<Telework>());
		}

		var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
		if (employee == null)
		{
			ViewBag.ErrorMessage = "Aucun employé trouvé pour votre compte. Veuillez contacter les RH.";
			return View("History", Enumerable.Empty<Telework>());
		}

		var items = await _service.GetAllAsync();
		var myItems = items.Where(t => t.EmployeeId == employee.Id).ToList();
		return View("History", myItems);
=======
        var items = await _service.GetAllAsync();
        return View("History", items);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    }
}



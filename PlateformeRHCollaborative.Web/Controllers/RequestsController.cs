using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Hubs;
using PlateformeRHCollaborative.Web.Services;
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Data;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize(Roles = "Manager,Directeur,RH")]
=======
using PlateformeRHCollaborative.Web.Hubs;
using PlateformeRHCollaborative.Web.Services;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize(Roles = "Manager")]
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
public class RequestsController : Controller
{
    private readonly LeaveService _leaveService;
    private readonly TeleworkService _teleworkService;
    private readonly IHubContext<NotificationsHub> _hubContext;
<<<<<<< HEAD
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d

    public RequestsController(
        LeaveService leaveService, 
        TeleworkService teleworkService,
<<<<<<< HEAD
        IHubContext<NotificationsHub> hubContext,
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context)
=======
        IHubContext<NotificationsHub> hubContext)
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    {
        _leaveService = leaveService;
        _teleworkService = teleworkService;
        _hubContext = hubContext;
<<<<<<< HEAD
        _userManager = userManager;
        _context = context;
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    }

    public async Task<IActionResult> Index()
    {
<<<<<<< HEAD
        var userId = _userManager.GetUserId(User);
        var currentEmployee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);

        if (currentEmployee == null)
        {
            return View("Error", new ErrorViewModel { RequestId = "User not found as Employee" });
        }

        var leaves = await _leaveService.GetAllAsync();
        var teleworks = await _teleworkService.GetAllAsync();

        // Filtrer selon la hiérarchie :
        // Le manager (ou directeur) ne voit que les demandes des employés dont il est le Manager direct.
        // Cela couvre :
        // - Manager IT voit les employés IT (leur ManagerId == ManagerIT.Id)
        // - Directeur voit RH et Managers (leur ManagerId == Directeur.Id)
        
        var myTeamLeaves = leaves.Where(l => l.Employee?.ManagerId == currentEmployee.Id).ToList();
        var myTeamTeleworks = teleworks.Where(t => t.Employee?.ManagerId == currentEmployee.Id).ToList();

        var pendingLeaves = myTeamLeaves.Where(l => l.Status == "Pending").ToList();
        var pendingTeleworks = myTeamTeleworks.Where(t => t.Status == "Pending").ToList();
=======
        var leaves = await _leaveService.GetAllAsync();
        var teleworks = await _teleworkService.GetAllAsync();

        var pendingLeaves = leaves.Where(l => l.Status == "Pending").ToList();
        var pendingTeleworks = teleworks.Where(t => t.Status == "Pending").ToList();
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d

        ViewBag.PendingLeaves = pendingLeaves;
        ViewBag.PendingTeleworks = pendingTeleworks;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApproveLeave(int id)
    {
<<<<<<< HEAD
        try
        {
            var actorId = await GetCurrentEmployeeIdAsync();
            await _leaveService.ApproveAsync(id, actorId);
            
            TempData["SuccessMessage"] = "Demande de congé approuvée.";
            // Notification TODO: Implement targeted notification
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            TempData["ErrorMessage"] = "Une erreur est survenue.";
        }
=======
        var leave = await _leaveService.GetByIdAsync(id);
        if (leave == null)
        {
            return NotFound();
        }

        leave.Status = "Approved";
        await _leaveService.UpdateAsync(leave);

        // Notification temps réel à l'employé
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
            $"Votre demande de congé du {leave.StartDate:dd/MM/yyyy} au {leave.EndDate:dd/MM/yyyy} a été approuvée.");

        TempData["SuccessMessage"] = "Demande de congé approuvée avec succès.";
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
<<<<<<< HEAD
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectLeave(int id, string rejectionReason)
    {
        try
        {
            var actorId = await GetCurrentEmployeeIdAsync();
            await _leaveService.RejectAsync(id, actorId, rejectionReason);
            
            TempData["SuccessMessage"] = "Demande de congé refusée.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
             Console.WriteLine(ex);
             TempData["ErrorMessage"] = "Une erreur est survenue.";
        }
=======
    public async Task<IActionResult> RejectLeave(int id, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            TempData["ErrorMessage"] = "Le motif de refus est obligatoire.";
            return RedirectToAction(nameof(Index));
        }

        var leave = await _leaveService.GetByIdAsync(id);
        if (leave == null)
        {
            return NotFound();
        }

        leave.Status = "Rejected";
        leave.RejectionReason = rejectionReason;
        await _leaveService.UpdateAsync(leave);

        // Notification temps réel à l'employé
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
            $"Votre demande de congé du {leave.StartDate:dd/MM/yyyy} au {leave.EndDate:dd/MM/yyyy} a été refusée. Motif : {rejectionReason}");

        TempData["SuccessMessage"] = "Demande de congé refusée.";
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveTelework(int id)
    {
<<<<<<< HEAD
        try
        {
            var actorId = await GetCurrentEmployeeIdAsync();
            await _teleworkService.ApproveAsync(id, actorId);
            
            TempData["SuccessMessage"] = "Demande de télétravail approuvée.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
=======
        var telework = await _teleworkService.GetByIdAsync(id);
        if (telework == null)
        {
            return NotFound();
        }

        telework.Status = "Approved";
        await _teleworkService.UpdateAsync(telework);

        // Notification temps réel à l'employé
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
            $"Votre demande de télétravail du {telework.StartDate:dd/MM/yyyy} au {telework.EndDate:dd/MM/yyyy} a été approuvée.");

        TempData["SuccessMessage"] = "Demande de télétravail approuvée avec succès.";
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
<<<<<<< HEAD
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectTelework(int id, string rejectionReason)
    {
        try
        {
            var actorId = await GetCurrentEmployeeIdAsync();
            await _teleworkService.RejectAsync(id, actorId, rejectionReason);
            
            TempData["SuccessMessage"] = "Demande de télétravail refusée.";
        }
        catch (BusinessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<int> GetCurrentEmployeeIdAsync()
    {
        var userId = _userManager.GetUserId(User);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        return employee?.Id ?? 0;
=======
    public async Task<IActionResult> RejectTelework(int id, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            TempData["ErrorMessage"] = "Le motif de refus est obligatoire.";
            return RedirectToAction(nameof(Index));
        }

        var telework = await _teleworkService.GetByIdAsync(id);
        if (telework == null)
        {
            return NotFound();
        }

        telework.Status = "Rejected";
        telework.RejectionReason = rejectionReason;
        await _teleworkService.UpdateAsync(telework);

        // Notification temps réel à l'employé
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
            $"Votre demande de télétravail du {telework.StartDate:dd/MM/yyyy} au {telework.EndDate:dd/MM/yyyy} a été refusée. Motif : {rejectionReason}");

        TempData["SuccessMessage"] = "Demande de télétravail refusée.";
        return RedirectToAction(nameof(Index));
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    }
}


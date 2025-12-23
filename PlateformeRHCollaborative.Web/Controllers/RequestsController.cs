using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Hubs;
using PlateformeRHCollaborative.Web.Services;
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Data;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize(Roles = "Manager,Directeur,RH")]
public class RequestsController : Controller
{
    private readonly LeaveService _leaveService;
    private readonly TeleworkService _teleworkService;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public RequestsController(
        LeaveService leaveService, 
        TeleworkService teleworkService,
        IHubContext<NotificationsHub> hubContext,
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context)
    {
        _leaveService = leaveService;
        _teleworkService = teleworkService;
        _hubContext = hubContext;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
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

        ViewBag.PendingLeaves = pendingLeaves;
        ViewBag.PendingTeleworks = pendingTeleworks;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApproveLeave(int id)
    {
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveTelework(int id)
    {
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlateformeRHCollaborative.Web.Hubs;
using PlateformeRHCollaborative.Web.Services;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize(Roles = "Manager")]
public class RequestsController : Controller
{
    private readonly LeaveService _leaveService;
    private readonly TeleworkService _teleworkService;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public RequestsController(
        LeaveService leaveService, 
        TeleworkService teleworkService,
        IHubContext<NotificationsHub> hubContext)
    {
        _leaveService = leaveService;
        _teleworkService = teleworkService;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        var leaves = await _leaveService.GetAllAsync();
        var teleworks = await _teleworkService.GetAllAsync();

        var pendingLeaves = leaves.Where(l => l.Status == "Pending").ToList();
        var pendingTeleworks = teleworks.Where(t => t.Status == "Pending").ToList();

        ViewBag.PendingLeaves = pendingLeaves;
        ViewBag.PendingTeleworks = pendingTeleworks;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApproveLeave(int id)
    {
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ApproveTelework(int id)
    {
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using PlateformeRHCollaborative.Web.Models.ViewModels;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public SettingsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var model = new UserSettingsViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            Has2FA = user.TwoFactorEnabled,
            Language = "Français", // Par défaut ou stocké ailleurs
            TimeZone = "Europe/Paris"
        };
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(UserSettingsViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        // On ne recharge pas tout le modèle ici si on veut juste retouner la vue avec erreur
        // Mais pour simplifier, on reconstruit les infos de base
        model.Username = user.UserName;
        model.Email = user.Email;
        model.Has2FA = user.TwoFactorEnabled;

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
        {
             ModelState.AddModelError(string.Empty, "Les mots de passe sont requis.");
             return View("Index", model);
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("Index", model);
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["SuccessMessage"] = "Votre mot de passe a été modifié avec succès.";
        return RedirectToAction(nameof(Index));
    }
}



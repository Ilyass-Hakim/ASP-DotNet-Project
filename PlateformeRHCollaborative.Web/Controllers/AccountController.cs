using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PlateformeRHCollaborative.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    /// <summary>
    /// S'assure que le rôle existe dans la base de données
    /// </summary>
    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    /// <summary>
    /// Extrait un nom à partir d'un email (partie avant @)
    /// </summary>
    private string ExtractNameFromEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "Utilisateur";

        var emailParts = email.Split('@');
        if (emailParts.Length > 0)
        {
            var namePart = emailParts[0];
            // Si le nom contient un point, prendre la première partie
            if (namePart.Contains('.'))
            {
                namePart = namePart.Split('.')[0];
            }
            // Capitaliser la première lettre
            if (namePart.Length > 0)
            {
                return char.ToUpper(namePart[0]) + namePart.Substring(1).ToLower();
            }
        }
        return "Utilisateur";
    }

    /// <summary>
    /// Redirige l'utilisateur vers le dashboard approprié selon son rôle
    /// </summary>
    /// <summary>
    /// Redirige l'utilisateur vers le dashboard approprié selon son rôle
    /// </summary>
    private async Task<IActionResult> RedirectToRoleDashboardAsync(IdentityUser user)
    {
        // Récupérer les rôles de l'utilisateur
        var userRoles = await _userManager.GetRolesAsync(user);
        
        // Redirection directe selon le rôle - ignorer returnUrl comme demandé
        if (userRoles.Contains("Directeur"))
        {
            return Redirect("/Manager/Home"); // Le Directeur utilise le même dashboard que les managers pour l'instant ou un spécifique si besoin
            // Note: Le user request dit "Quand il demande un congé, il s'auto-approuve".
            // Il agit comme un super-manager.
        }
        else if (userRoles.Contains("RH"))
        {
            return Redirect("/RH/Home");
        }
        else if (userRoles.Contains("Manager"))
        {
            return Redirect("/Manager/Home");
        }
        else if (userRoles.Contains("Employe"))
        {
            return Redirect("/Employe/Home");
        }

        // Par défaut, rediriger vers l'accueil
        return Redirect("/Home/Index");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        // Validation des champs
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.ErrorMessage = "Veuillez remplir tous les champs.";
            return View();
        }

        // Rechercher l'utilisateur
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            ViewBag.ErrorMessage = "Identifiants invalides";
            return View();
        }

        // Authentification
        var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: true);
        
        if (result.Succeeded)
        {
            return await RedirectToRoleDashboardAsync(user);
        }
        
        if (result.IsLockedOut)
        {
            ViewBag.ErrorMessage = "Votre compte a été verrouillé. Veuillez réessayer plus tard.";
            return View();
        }

        ViewBag.ErrorMessage = "Identifiants invalides";
        return View();
    }

    /// <summary>
    /// Page de création de compte - Accessible uniquement à RH
    /// </summary>
    [Authorize(Roles = "RH")]
    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Departments = new List<string> { "IT", "Marketing", "Comptabilité", "Production", "Commercial", "RH", "Direction" };
        return View();
    }

    /// <summary>
    /// Création d'un compte - Accessible uniquement à RH
    /// </summary>
    [Authorize(Roles = "RH")]
    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string confirmPassword, string role, string department)
    {
        ViewBag.Departments = new List<string> { "IT", "Marketing", "Comptabilité", "Production", "Commercial", "RH", "Direction" };

        // Validation des champs
        if (string.IsNullOrWhiteSpace(email))
        {
            ViewBag.ErrorMessage = "L'email est requis.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ViewBag.ErrorMessage = "Le mot de passe est requis.";
            return View();
        }

        if (password != confirmPassword)
        {
            ViewBag.ErrorMessage = "Les mots de passe ne correspondent pas.";
            return View();
        }

        // Validation du rôle
        if (string.IsNullOrWhiteSpace(role) || (role != "Employe" && role != "Manager" && role != "RH" && role != "Directeur"))
        {
            ViewBag.ErrorMessage = "Veuillez sélectionner un rôle valide.";
            return View();
        }

        // Vérifier si l'utilisateur existe déjà
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            ViewBag.ErrorMessage = "Un compte avec cet email existe déjà.";
            return View();
        }

        // S'assurer que le rôle existe
        await EnsureRoleExistsAsync(role);

        // Création de l'utilisateur Identity
        var user = new IdentityUser 
        { 
            UserName = email, 
            Email = email 
        };
        
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Assigner le rôle sélectionné
            await _userManager.AddToRoleAsync(user, role);

            // Logique d'assignation du Manager et Département
            int? managerId = null;

            // Trouver le Directeur (pour RH et Managers)
            var director = await _context.Employees.FirstOrDefaultAsync(e => e.Role == "Directeur");

            if (role == "Directeur")
            {
                // Le Directeur n'a pas de manager
                managerId = null;
                department = "Direction";
            }
            else if (role == "RH")
            {
                // Le RH rapporte au Directeur
                managerId = director?.Id;
                department = "RH";
            }
            else if (role == "Manager")
            {
                // Les Managers rapportent au Directeur
                managerId = director?.Id;
                // Le département est celui sélectionné
            }
            else if (role == "Employe")
            {
                // Les employés rapportent au Manager de leur département
                var departmentManager = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Role == "Manager" && e.Department == department);
                
                managerId = departmentManager?.Id;
            }

            // Créer automatiquement l'enregistrement Employee correspondant
            var employee = new Employee
            {
                UserId = user.Id,
                Email = email,
                Nom = ExtractNameFromEmail(email),
                Poste = "À définir", 
                Role = role,
                Department = department ?? "Non assigné",
                ManagerId = managerId,
                SoldeConges = 25
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Message de succès avec détails
            string managerInfo = managerId.HasValue ? "Manager assigné automatiquement." : (role == "Directeur" ? "Compte Directeur (pas de manager)." : "ATTENTION: Aucun manager trouvé pour ce poste !");
            ViewBag.SuccessMessage = $"Compte créé avec succès pour {email} ({role} - {department}). {managerInfo}";
            return View();
        }

        // Gestion des erreurs de validation
        var errors = string.Join(" ", result.Errors.Select(e => e.Description));
        ViewBag.ErrorMessage = $"Erreur lors de la création du compte : {errors}";
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

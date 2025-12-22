using System.ComponentModel.DataAnnotations;

namespace PlateformeRHCollaborative.Web.Models.ViewModels
{
    public class UserSettingsViewModel
    {
        // Account Info
        public string? Username { get; set; }
        public string? Email { get; set; }

        // Password Change
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        [StringLength(100, ErrorMessage = "Le {0} doit comporter au moins {2} caractères.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule et un chiffre.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et la confirmation ne correspondent pas.")]
        public string? ConfirmPassword { get; set; }

        // Preferences
        public bool EmailNotifications { get; set; } = true;
        public string Language { get; set; } = "fr";
        public string TimeZone { get; set; } = "Europe/Paris";
        
        // Security
        public bool Has2FA { get; set; }
    }
}

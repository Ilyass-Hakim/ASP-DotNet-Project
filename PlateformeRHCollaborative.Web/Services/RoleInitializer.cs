using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;

namespace PlateformeRHCollaborative.Web.Services;

public class RoleInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public RoleInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

<<<<<<< HEAD
            // Hack: Ajout manuel des colonnes car nous sommes sur SQL Server et les migrations sont bloquées
            try {
                await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Employees ADD SalaryBase decimal(18,2) NOT NULL DEFAULT 3000", cancellationToken);
            } catch { /* Ignorer si déjà existant */ }

            // Attendre que la base de données soit prête
            try {
                await dbContext.Database.MigrateAsync(cancellationToken);
            } catch (Exception ex) { 
                Console.WriteLine($"Migration error (ignoring): {ex.Message}");
            }

            string[] roles = { "Employe", "Manager", "RH", "Directeur" };
=======
            // Attendre que la base de données soit prête
            await dbContext.Database.MigrateAsync(cancellationToken);

            string[] roles = { "Employe", "Manager", "RH" };
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
<<<<<<< HEAD

            // Seed Admin User
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var adminEmail = "admin@gmail.com";
            
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser 
                { 
                    UserName = adminEmail, 
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(adminUser, "Yasmine123*");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "RH");
                    
                    // Create associated Employee record
                    var employee = new PlateformeRHCollaborative.Web.Models.Employee
                    {
                        UserId = adminUser.Id,
                        Nom = "Admin RH", // Legacy compatible
                        FirstName = "Admin",
                        LastName = "RH",
                        Email = adminEmail,
                        Role = "RH",
                        Department = "RH",
                        Matricule = "RH001",
                        Poste = "Administrateur RH",
                        HireDate = DateTime.Today,
                        IsActive = true,
                        SalaryBase = 4000
                    };
                    
                    dbContext.Employees.Add(employee);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
        }
        catch (Exception ex)
        {
            // Logger l'erreur mais ne pas bloquer le démarrage
<<<<<<< HEAD
            Console.WriteLine($"Erreur lors de l'initialisation des rôles : {ex}");
=======
            Console.WriteLine($"Erreur lors de l'initialisation des rôles : {ex.Message}");
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}


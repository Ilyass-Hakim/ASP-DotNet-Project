# Résumé des modifications pour résoudre le problème de démarrage

## Problèmes identifiés et corrigés

### 1. Configuration des ports (launchSettings.json)

**Problème** : Les ports 5063 et 7195 étaient probablement en conflit avec d'autres applications.

**Solution** : Modification des ports vers :
- **HTTP** : `http://localhost:5085`
- **HTTPS** : `https://localhost:7215`

### 2. Certificats HTTPS corrompus

**Problème** : Les certificats de développement HTTPS étaient corrompus ou invalides.

**Solution** : Régénération des certificats avec :
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### 3. Code asynchrone bloquant dans Program.cs

**Problème** : Le code de création des rôles utilisait `await` avant `app.Run()`, ce qui bloquait le démarrage de l'application.

**Solution** : Déplacement de la logique de création des rôles vers un `IHostedService` (`RoleInitializer`) qui s'exécute de manière asynchrone après le démarrage de l'application.

### 4. Vérifications de null dans les vues

**Problème** : Avertissements CS8602 pour des références potentiellement null dans `_Layout.cshtml`.

**Solution** : Ajout de vérifications de null avec l'opérateur `?` :
- `User.Identity?.IsAuthenticated == true`
- `@(User.Identity?.Name ?? "Utilisateur")`

### 5. Service d'initialisation des rôles

**Nouveau fichier** : `Services/RoleInitializer.cs`

Ce service crée automatiquement les rôles suivants au démarrage :
- **Employé**
- **Manager**
- **RH**

Le service gère également les migrations de base de données et inclut une gestion d'erreurs robuste pour ne pas bloquer le démarrage de l'application.

## Fichiers modifiés

1. **Properties/launchSettings.json** - Changement des ports
2. **Program.cs** - Correction du middleware et ajout du service RoleInitializer
3. **Views/Shared/_Layout.cshtml** - Correction des vérifications de null
4. **Services/RoleInitializer.cs** - Nouveau fichier pour l'initialisation des rôles
5. **README_DEPLOIEMENT.md** - Nouveau fichier avec instructions de déploiement

## Commandes de démarrage

### Première fois (ou après nettoyage)
```bash
# Nettoyer les certificats
dotnet dev-certs https --clean

# Générer et approuver les certificats
dotnet dev-certs https --trust

# Construire le projet
dotnet build

# Lancer l'application
dotnet run
```

### Démarrages suivants
```bash
dotnet run
```

## Accès à l'application

Une fois démarrée, l'application est accessible sur :
- **HTTPS** : https://localhost:7215 (recommandé)
- **HTTP** : http://localhost:5085

## Vérification

Pour vérifier que l'application fonctionne correctement :
1. Exécutez `dotnet run`
2. Ouvrez votre navigateur sur https://localhost:7215
3. Vous devriez voir la page d'accueil de CollabRH
4. Les rôles sont automatiquement créés au premier démarrage

## Dépannage

### L'application ne démarre pas
- Vérifiez que SQL Server LocalDB est installé et démarré
- Exécutez `sqllocaldb start mssqllocaldb` pour démarrer LocalDB
- Vérifiez les logs de l'application pour les erreurs

### Erreur de certificat HTTPS
- Réexécutez `dotnet dev-certs https --trust`
- Cliquez "Oui" dans la boîte de dialogue UAC si elle apparaît

### Port déjà utilisé
- Vérifiez qu'aucune autre application n'utilise les ports 5085 ou 7215
- Modifiez les ports dans `launchSettings.json` si nécessaire

## Prochaines étapes

1. Créez un compte utilisateur via la page d'inscription
2. Sélectionnez un rôle (Employé, Manager ou RH)
3. Explorez les fonctionnalités de l'application selon votre rôle



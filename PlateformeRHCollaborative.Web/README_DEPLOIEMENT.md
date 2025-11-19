# CollabRH - Instructions de démarrage

## Configuration des ports

Le projet est configuré pour utiliser les ports suivants :
- **HTTP** : http://localhost:5085
- **HTTPS** : https://localhost:7215

## Démarrage de l'application

### 1. Vérifier la base de données

Assurez-vous que SQL Server LocalDB est installé et démarré.

### 2. Régénérer les certificats HTTPS (si nécessaire)

Si vous rencontrez des problèmes avec les certificats HTTPS, exécutez :

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### 3. Lancer l'application

```bash
dotnet run
```

L'application démarre automatiquement sur :
- https://localhost:7215 (HTTPS - par défaut)
- http://localhost:5085 (HTTP)

### 4. Accéder à l'application

Ouvrez votre navigateur et accédez à :
- **https://localhost:7215** (recommandé)
- ou **http://localhost:5085**

## Rôles utilisateurs

L'application crée automatiquement les rôles suivants au démarrage :
- **Employé** : Accès aux fonctionnalités de base (Congés, Télétravail)
- **Manager** : Accès aux fonctionnalités de base + Documents RH
- **RH** : Accès complet + Tableau de bord RH avec statistiques

## Création d'un compte

Lors du premier démarrage, vous pouvez créer un compte via la page d'inscription :
1. Cliquez sur "Créer un compte"
2. Entrez votre email et mot de passe
3. Sélectionnez votre rôle
4. Cliquez sur "Créer mon compte"

## Dépannage

### Problème de certificat HTTPS

Si vous voyez une erreur de certificat, exécutez :
```bash
dotnet dev-certs https --trust
```

### Problème de port déjà utilisé

Si les ports 5085 ou 7215 sont déjà utilisés, modifiez le fichier `Properties/launchSettings.json` et changez les ports.

### Problème de base de données

Vérifiez que SQL Server LocalDB est en cours d'exécution :
```bash
sqllocaldb info
sqllocaldb start mssqllocaldb
```

## Technologies utilisées

- ASP.NET Core 9.0
- Entity Framework Core
- ASP.NET Identity
- SignalR
- Bootstrap 5
- Chart.js
- FontAwesome

## Licence

© 2025 CollabRH. Tous droits réservés.



# 🏢 CollabRH - Plateforme RH Collaborative

Une application web moderne de gestion des ressources humaines développée avec ASP.NET Core 9.0, offrant une solution complète pour la gestion des congés, du télétravail, des documents RH et bien plus.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-purple)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-blue)
![License](https://img.shields.io/badge/License-Proprietary-red)

## ✨ Fonctionnalités

### 👤 Pour les Employés
- 📅 Gestion des demandes de congés
- 🏠 Demandes de télétravail
- 📊 Suivi des absences et présences
- 📄 Consultation des documents RH
- 🔔 Notifications en temps réel

### 👔 Pour les Managers
- ✅ Validation des demandes de congés
- 📈 Vue d'ensemble de l'équipe
- 📑 Accès aux documents RH
- 👥 Gestion des membres de l'équipe

### 🎯 Pour les RH
- 📊 Tableau de bord avec statistiques complètes
- 👨‍💼 Gestion des employés
- 📋 Gestion des départements
- 📈 Rapports et analyses
- 📤 Gestion des documents RH

### 🏆 Pour les Directeurs
- 📊 Vue stratégique globale
- 💼 Accès à toutes les fonctionnalités
- 📈 Analyses et rapports détaillés

## 🚀 Technologies utilisées

- **Backend**: ASP.NET Core 9.0
- **ORM**: Entity Framework Core
- **Authentification**: ASP.NET Identity
- **Base de données**: SQL Server LocalDB
- **Temps réel**: SignalR
- **Frontend**: Bootstrap 5, Chart.js
- **Icônes**: FontAwesome

## 📋 Prérequis

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- Un navigateur web moderne

## 🔧 Installation

### 1. Cloner le repository

```bash
git clone https://github.com/votre-username/collabrh.git
cd collabrh
```

### 2. Restaurer les packages NuGet

```bash
dotnet restore
```

### 3. Configurer la base de données

La base de données sera créée automatiquement au premier lancement. Un fichier de sauvegarde `PlateformeRHCollaborative.bacpac` est également fourni pour une restauration rapide.

```bash
dotnet ef database update
```

### 4. Configurer les certificats HTTPS

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### 5. Lancer l'application

```bash
dotnet run
```

L'application sera accessible sur :
- 🔒 **HTTPS** : https://localhost:7215 (recommandé)
- 🌐 **HTTP** : http://localhost:5085

## 👥 Comptes de démonstration

| Rôle | Email | Mot de passe |
|------|-------|--------------|
| 👨‍💼 **Employé** | `Employe_IT1@gmail.com` | `Employeit123*` |
| 👔 **Manager** | `managerIT@gmail.com` | `Managerit123*` |
| 🎯 **RH** | `admin@gmail.com` | `Admin123*` |
| 🏆 **Directeur** | `pdg@gmail.com` | `Pdg1234*` |

## 📁 Structure du projet

```
CollabRH/
├── Controllers/          # Contrôleurs MVC
├── Models/              # Modèles de données
├── Views/               # Vues Razor
├── Data/                # DbContext et migrations
├── Services/            # Services métier
├── wwwroot/             # Fichiers statiques (CSS, JS, images)
└── Properties/          # Configuration de l'application
```

## ⚙️ Configuration

### Ports

Les ports par défaut sont configurés dans `Properties/launchSettings.json` :
- HTTP : 5085
- HTTPS : 7215

Pour modifier les ports, éditez le fichier `launchSettings.json`.

### Base de données

La chaîne de connexion est configurée dans `appsettings.json`. Par défaut, elle utilise SQL Server LocalDB.

## 🔍 Dépannage

### ❌ Erreur de certificat HTTPS

```bash
dotnet dev-certs https --trust
```

### ❌ Port déjà utilisé

Modifiez les ports dans `Properties/launchSettings.json`

### ❌ Problème de base de données

Vérifiez que LocalDB est en cours d'exécution :

```bash
sqllocaldb info
sqllocaldb start mssqllocaldb
```

### 🔄 Restaurer la base de données

Pour restaurer depuis le fichier `.bacpac` :
1. Ouvrez SQL Server Management Studio (SSMS)
2. Clic droit sur "Databases" → "Import Data-tier Application"
3. Sélectionnez le fichier `PlateformeRHCollaborative.bacpac`

## 🤝 Contribution

Les contributions sont les bienvenues ! Pour contribuer :

1. Forkez le projet
2. Créez votre branche (`git checkout -b feature/AmazingFeature`)
3. Committez vos changements (`git commit -m 'Add some AmazingFeature'`)
4. Poussez vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrez une Pull Request

## 📝 Licence

© 2025 CollabRH. Tous droits réservés.

## 📧 Contact

Pour toute question ou suggestion, n'hésitez pas à ouvrir une issue sur GitHub.

---

⭐ Si vous aimez ce projet, n'hésitez pas à lui donner une étoile sur GitHub !

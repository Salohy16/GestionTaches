# Gestion des taches des employes - Projet WinForms C#

## 1. Corriger la base de donnees (OBLIGATOIRE)

Ouvre phpMyAdmin sur ta base `bd_tache` et execute le script
`00_fix_bd_tache.sql` fourni dans ce dossier (onglet SQL, coller/executer).

Ce script corrige :
- `commentaire.texte` (etait en INT -> passe en TEXT)
- `commentaire.date` (etait en INT -> renommee `date_commentaire` et passee en DATE)
- ajoute la cle etrangere manquante entre `commentaire.employe` et `employe`
- passe les ID en AUTO_INCREMENT (employe, tache, commentaire, notification)
- ajoute la colonne `tache.statut` (indispensable pour le suivi d'avancement demande dans l'enonce)
- passe `notification.lu` en booleen
- ajoute le ON DELETE CASCADE entre tache et employe

Sans ce script, l'application ne fonctionnera pas (le code C# utilise
ces colonnes/noms corriges).

## 2. Ouvrir le projet dans Visual Studio 2026

- Ouvre directement le fichier `GestionTaches.csproj` avec Visual Studio
  (Fichier > Ouvrir > Projet/Solution).
- Visual Studio va restaurer automatiquement le package NuGet `MySqlConnector`
  au premier chargement (verifie que tu as internet). Sinon : clic droit sur
  le projet > "Gerer les packages NuGet" > rechercher "MySqlConnector" > Installer.

## 3. Configurer la connexion a la base

Ouvre `Data/DatabaseHelper.cs` et adapte si besoin :

```csharp
private const string Server = "127.0.0.1";
private const string Port = "3306";
private const string Database = "bd_tache";
private const string User = "root";
private const string Password = ""; // ton mot de passe MySQL
```

## 4. Lancer (F5)

L'application s'ouvre sur un tableau de bord listant toutes les taches,
avec des boutons vers :
- **Gerer les taches** : creation/modification/suppression, attribution
  a un employe, priorite, date limite, statut (suivi d'avancement),
  acces direct aux commentaires et a l'historique de la tache
- **Gerer les employes** : CRUD employes
- **Notifications** : notifications generees automatiquement a la creation
  d'une tache, au changement de statut ou de responsable
- **Historique** : toutes les actions loggees automatiquement (creation,
  modification, changement de statut, ajout de commentaire)
- **Rapport de productivite** : nombre de taches par employe, taches
  terminees, taches en retard, taux de completion

## Structure du projet

```
GestionTaches/
  Program.cs                 point d'entree
  Models/                    classes representant les tables (POCO)
  Data/                      acces BD (DAO) - une classe par table
  Forms/                     interfaces WinForms
  00_fix_bd_tache.sql        script de correction de la BD
```

## Pistes d'amelioration si tu as du temps en plus

- Ajouter un filtre/tri dans la liste des taches (par employe, priorite, retard)
- Ajouter un export du rapport en PDF ou Excel
- Ajouter un formulaire de connexion (login employe)
- Colorer en rouge les lignes en retard dans le DataGridView

using GestionTaches.Data;
using GestionTaches.Models;

namespace GestionTaches.Forms;

public class MainForm : Form
{
    private DataGridView _grid = null!;
    private Label _lblTitre = null!;
    private readonly TacheDAO _tacheDAO = new();

    public MainForm()
    {
        Text = "Gestion des taches des employes";
        Width = 1000;
        Height = 650;
        StartPosition = FormStartPosition.CenterScreen;

        ConstruireUI();
        ChargerTaches();
    }

    private void ConstruireUI()
    {
        _lblTitre = new Label
        {
            Text = "Tableau de bord",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 40,
            Padding = new Padding(10, 10, 0, 0)
        };

        var panelBoutons = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 50,
            Padding = new Padding(10)
        };

        var btnTaches = new Button { Text = "Gerer les taches", Width = 150, Height = 32 };
        var btnEmployes = new Button { Text = "Gerer les employes", Width = 150, Height = 32 };
        var btnNotifications = new Button { Text = "Notifications", Width = 150, Height = 32 };
        var btnHistorique = new Button { Text = "Historique", Width = 150, Height = 32 };
        var btnRapport = new Button { Text = "Rapport de productivite", Width = 180, Height = 32 };
        var btnActualiser = new Button { Text = "Actualiser", Width = 100, Height = 32 };

        btnTaches.Click += (s, e) => { new FormTaches().ShowDialog(); ChargerTaches(); };
        btnEmployes.Click += (s, e) => { new FormEmployes().ShowDialog(); ChargerTaches(); };
        btnNotifications.Click += (s, e) => new FormNotifications().ShowDialog();
        btnHistorique.Click += (s, e) => new FormHistorique().ShowDialog();
        btnRapport.Click += (s, e) => new FormRapport().ShowDialog();
        btnActualiser.Click += (s, e) => ChargerTaches();

        panelBoutons.Controls.AddRange(new Control[]
        {
            btnTaches, btnEmployes, btnNotifications, btnHistorique, btnRapport, btnActualiser
        });

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        Controls.Add(_grid);
        Controls.Add(panelBoutons);
        Controls.Add(_lblTitre);
    }

    private void ChargerTaches()
    {
        var taches = _tacheDAO.GetAll();

        var source = taches.Select(t => new
        {
            t.IdTache,
            Titre = t.Titre,
            Employe = t.NomEmploye,
            Priorite = Tache.PrioriteLibelle(t.Priorite),
            Statut = t.Statut,
            DateLimite = t.DateLimite.ToShortDateString(),
            EnRetard = t.EstEnRetard ? "OUI" : ""
        }).ToList();

        _grid.DataSource = source;

        if (_grid.Columns["IdTache"] != null)
            _grid.Columns["IdTache"].HeaderText = "ID";
    }
}

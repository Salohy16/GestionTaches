using GestionTaches.Data;
using GestionTaches.Models;

namespace GestionTaches.Forms;

public class FormTaches : Form
{
    private readonly TacheDAO _tacheDAO = new();
    private readonly EmployeDAO _employeDAO = new();

    private DataGridView _grid = null!;
    private TextBox _txtTitre = null!, _txtDescription = null!;
    private ComboBox _cmbEmploye = null!, _cmbPriorite = null!, _cmbStatut = null!;
    private DateTimePicker _dtpLimite = null!;
    private Tache? _tacheSelectionnee = null;

    public FormTaches()
    {
        Text = "Gestion des taches";
        Width = 1050;
        Height = 650;
        StartPosition = FormStartPosition.CenterParent;
        ConstruireUI();
        Charger();
    }

    private void ConstruireUI()
    {
        var panelForm = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 220,
            ColumnCount = 4,
            Padding = new Padding(10)
        };
        for (int i = 0; i < 4; i++)
            panelForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        _txtTitre = new TextBox { Dock = DockStyle.Fill };
        _txtDescription = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
        _cmbEmploye = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Nom", ValueMember = "IdEmploye" };
        // Affichage personnalise "Prenom Nom"
        _cmbEmploye.Format += (s, e) => { if (e.ListItem is Employe emp) e.Value = emp.ToString(); };

        _cmbPriorite = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbPriorite.Items.AddRange(new object[] { "1 - Basse", "2 - Moyenne", "3 - Haute" });

        _cmbStatut = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbStatut.Items.AddRange(new object[] { "A faire", "En cours", "Terminee" });

        _dtpLimite = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };

        panelForm.Controls.Add(new Label { Text = "Titre", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        panelForm.Controls.Add(_txtTitre, 1, 0);
        panelForm.Controls.Add(new Label { Text = "Employe", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 0);
        panelForm.Controls.Add(_cmbEmploye, 3, 0);

        panelForm.Controls.Add(new Label { Text = "Description", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        panelForm.SetColumnSpan(_txtDescription, 3);
        panelForm.Controls.Add(_txtDescription, 1, 1);

        panelForm.Controls.Add(new Label { Text = "Priorite", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        panelForm.Controls.Add(_cmbPriorite, 1, 2);
        panelForm.Controls.Add(new Label { Text = "Statut", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 2);
        panelForm.Controls.Add(_cmbStatut, 3, 2);

        panelForm.Controls.Add(new Label { Text = "Date limite", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 3);
        panelForm.Controls.Add(_dtpLimite, 1, 3);

        var panelBoutons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(10) };
        var btnAjouter = new Button { Text = "Ajouter", Width = 100 };
        var btnModifier = new Button { Text = "Modifier", Width = 100 };
        var btnSupprimer = new Button { Text = "Supprimer", Width = 100 };
        var btnNouveau = new Button { Text = "Nouveau", Width = 100 };
        var btnCommentaires = new Button { Text = "Commentaires", Width = 120 };
        var btnHistorique = new Button { Text = "Historique de la tache", Width = 160 };

        btnAjouter.Click += (s, e) => Ajouter();
        btnModifier.Click += (s, e) => Modifier();
        btnSupprimer.Click += (s, e) => Supprimer();
        btnNouveau.Click += (s, e) => ViderFormulaire();
        btnCommentaires.Click += (s, e) => OuvrirCommentaires();
        btnHistorique.Click += (s, e) => OuvrirHistorique();

        panelBoutons.Controls.AddRange(new Control[]
        {
            btnAjouter, btnModifier, btnSupprimer, btnNouveau, btnCommentaires, btnHistorique
        });

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        _grid.SelectionChanged += (s, e) => SelectionnerLigne();

        Controls.Add(_grid);
        Controls.Add(panelBoutons);
        Controls.Add(panelForm);

        ChargerEmployesDansCombo();
    }

    private void ChargerEmployesDansCombo()
    {
        _cmbEmploye.DataSource = _employeDAO.GetAll();
    }

    private void Charger()
    {
        var taches = _tacheDAO.GetAll();
        _grid.DataSource = taches.Select(t => new
        {
            t.IdTache,
            t.Titre,
            Employe = t.NomEmploye,
            Priorite = Tache.PrioriteLibelle(t.Priorite),
            t.Statut,
            DateLimite = t.DateLimite.ToShortDateString(),
            EnRetard = t.EstEnRetard ? "OUI" : ""
        }).ToList();

        // On garde la liste complete pour retrouver l'objet Tache lors de la selection
        _tachesEnMemoire = taches;
    }

    private List<Tache> _tachesEnMemoire = new();

    private void SelectionnerLigne()
    {
        if (_grid.CurrentRow == null) return;
        int id = (int)_grid.CurrentRow.Cells["IdTache"].Value;
        var tache = _tachesEnMemoire.FirstOrDefault(t => t.IdTache == id);
        if (tache == null) return;

        _tacheSelectionnee = tache;
        _txtTitre.Text = tache.Titre;
        _txtDescription.Text = tache.Description;
        _cmbEmploye.SelectedValue = tache.IdEmploye;
        _cmbPriorite.SelectedIndex = tache.Priorite - 1;
        _cmbStatut.SelectedItem = tache.Statut;
        _dtpLimite.Value = tache.DateLimite;
    }

    private bool ValiderSaisie()
    {
        if (string.IsNullOrWhiteSpace(_txtTitre.Text))
        {
            MessageBox.Show("Le titre est obligatoire.");
            return false;
        }
        if (_cmbEmploye.SelectedValue == null)
        {
            MessageBox.Show("Selectionne un employe.");
            return false;
        }
        if (_cmbPriorite.SelectedIndex < 0)
        {
            MessageBox.Show("Selectionne une priorite.");
            return false;
        }
        if (_cmbStatut.SelectedIndex < 0)
        {
            MessageBox.Show("Selectionne un statut.");
            return false;
        }
        return true;
    }

    private void Ajouter()
    {
        if (!ValiderSaisie()) return;

        var tache = new Tache
        {
            Titre = _txtTitre.Text,
            Description = _txtDescription.Text,
            IdEmploye = (int)_cmbEmploye.SelectedValue!,
            Priorite = _cmbPriorite.SelectedIndex + 1,
            Statut = _cmbStatut.SelectedItem!.ToString()!,
            DateCreation = DateTime.Today,
            DateLimite = _dtpLimite.Value.Date
        };

        _tacheDAO.Insert(tache);
        Charger();
        ViderFormulaire();
    }

    private void Modifier()
    {
        if (_tacheSelectionnee == null)
        {
            MessageBox.Show("Selectionne d'abord une tache dans la liste.");
            return;
        }
        if (!ValiderSaisie()) return;

        int ancienEmploye = _tacheSelectionnee.IdEmploye;
        string ancienStatut = _tacheSelectionnee.Statut;

        var tache = new Tache
        {
            IdTache = _tacheSelectionnee.IdTache,
            Titre = _txtTitre.Text,
            Description = _txtDescription.Text,
            IdEmploye = (int)_cmbEmploye.SelectedValue!,
            Priorite = _cmbPriorite.SelectedIndex + 1,
            Statut = _cmbStatut.SelectedItem!.ToString()!,
            DateCreation = _tacheSelectionnee.DateCreation,
            DateLimite = _dtpLimite.Value.Date
        };

        _tacheDAO.Update(tache, ancienEmploye, ancienStatut);
        Charger();
        ViderFormulaire();
    }

    private void Supprimer()
    {
        if (_tacheSelectionnee == null)
        {
            MessageBox.Show("Selectionne d'abord une tache dans la liste.");
            return;
        }
        var confirmation = MessageBox.Show("Supprimer cette tache (et ses commentaires/historique) ?",
            "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirmation != DialogResult.Yes) return;

        _tacheDAO.Delete(_tacheSelectionnee.IdTache);
        Charger();
        ViderFormulaire();
    }

    private void OuvrirCommentaires()
    {
        if (_tacheSelectionnee == null)
        {
            MessageBox.Show("Selectionne d'abord une tache dans la liste.");
            return;
        }
        new FormCommentaires(_tacheSelectionnee.IdTache, _tacheSelectionnee.Titre).ShowDialog();
    }

    private void OuvrirHistorique()
    {
        if (_tacheSelectionnee == null)
        {
            MessageBox.Show("Selectionne d'abord une tache dans la liste.");
            return;
        }
        new FormHistorique(_tacheSelectionnee.IdTache).ShowDialog();
    }

    private void ViderFormulaire()
    {
        _tacheSelectionnee = null;
        _txtTitre.Text = "";
        _txtDescription.Text = "";
        if (_cmbEmploye.Items.Count > 0) _cmbEmploye.SelectedIndex = 0;
        _cmbPriorite.SelectedIndex = -1;
        _cmbStatut.SelectedIndex = 0;
        _dtpLimite.Value = DateTime.Today.AddDays(7);
    }
}

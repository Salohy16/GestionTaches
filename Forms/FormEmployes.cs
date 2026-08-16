using GestionTaches.Data;
using GestionTaches.Models;

namespace GestionTaches.Forms;

public class FormEmployes : Form
{
    private readonly EmployeDAO _dao = new();
    private DataGridView _grid = null!;
    private TextBox _txtNom = null!, _txtPrenom = null!, _txtMail = null!;
    private int _idSelectionne = 0;

    public FormEmployes()
    {
        Text = "Gestion des employes";
        Width = 700;
        Height = 500;
        StartPosition = FormStartPosition.CenterParent;
        ConstruireUI();
        Charger();
    }

    private void ConstruireUI()
    {
        var panelForm = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 160,
            ColumnCount = 2,
            Padding = new Padding(10)
        };
        panelForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
        panelForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _txtNom = new TextBox { Dock = DockStyle.Fill };
        _txtPrenom = new TextBox { Dock = DockStyle.Fill };
        _txtMail = new TextBox { Dock = DockStyle.Fill };

        panelForm.Controls.Add(new Label { Text = "Nom", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        panelForm.Controls.Add(_txtNom, 1, 0);
        panelForm.Controls.Add(new Label { Text = "Prenom", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
        panelForm.Controls.Add(_txtPrenom, 1, 1);
        panelForm.Controls.Add(new Label { Text = "Email", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 2);
        panelForm.Controls.Add(_txtMail, 1, 2);

        var panelBoutons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(10) };
        var btnAjouter = new Button { Text = "Ajouter", Width = 100 };
        var btnModifier = new Button { Text = "Modifier", Width = 100 };
        var btnSupprimer = new Button { Text = "Supprimer", Width = 100 };
        var btnVider = new Button { Text = "Nouveau", Width = 100 };

        btnAjouter.Click += (s, e) => Ajouter();
        btnModifier.Click += (s, e) => Modifier();
        btnSupprimer.Click += (s, e) => Supprimer();
        btnVider.Click += (s, e) => ViderFormulaire();

        panelBoutons.Controls.AddRange(new Control[] { btnAjouter, btnModifier, btnSupprimer, btnVider });

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
    }

    private void Charger()
    {
        _grid.DataSource = _dao.GetAll().Select(e => new
        {
            e.IdEmploye,
            e.Nom,
            e.Prenom,
            e.Mail
        }).ToList();
    }

    private void SelectionnerLigne()
    {
        if (_grid.CurrentRow == null) return;
        _idSelectionne = (int)_grid.CurrentRow.Cells["IdEmploye"].Value;
        _txtNom.Text = _grid.CurrentRow.Cells["Nom"].Value.ToString();
        _txtPrenom.Text = _grid.CurrentRow.Cells["Prenom"].Value.ToString();
        _txtMail.Text = _grid.CurrentRow.Cells["Mail"].Value.ToString();
    }

    private bool ValiderSaisie()
    {
        if (string.IsNullOrWhiteSpace(_txtNom.Text) || string.IsNullOrWhiteSpace(_txtPrenom.Text) || string.IsNullOrWhiteSpace(_txtMail.Text))
        {
            MessageBox.Show("Merci de remplir tous les champs.", "Champs manquants", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void Ajouter()
    {
        if (!ValiderSaisie()) return;
        _dao.Insert(new Employe { Nom = _txtNom.Text, Prenom = _txtPrenom.Text, Mail = _txtMail.Text });
        Charger();
        ViderFormulaire();
    }

    private void Modifier()
    {
        if (_idSelectionne == 0)
        {
            MessageBox.Show("Selectionne d'abord un employe dans la liste.");
            return;
        }
        if (!ValiderSaisie()) return;
        _dao.Update(new Employe { IdEmploye = _idSelectionne, Nom = _txtNom.Text, Prenom = _txtPrenom.Text, Mail = _txtMail.Text });
        Charger();
        ViderFormulaire();
    }

    private void Supprimer()
    {
        if (_idSelectionne == 0)
        {
            MessageBox.Show("Selectionne d'abord un employe dans la liste.");
            return;
        }
        var confirmation = MessageBox.Show(
            "Supprimer cet employe ? Ses taches, commentaires et notifications seront aussi supprimes.",
            "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirmation != DialogResult.Yes) return;

        _dao.Delete(_idSelectionne);
        Charger();
        ViderFormulaire();
    }

    private void ViderFormulaire()
    {
        _idSelectionne = 0;
        _txtNom.Text = "";
        _txtPrenom.Text = "";
        _txtMail.Text = "";
    }
}

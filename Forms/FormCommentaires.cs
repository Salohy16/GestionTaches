using GestionTaches.Data;
using GestionTaches.Models;

namespace GestionTaches.Forms;

public class FormCommentaires : Form
{
    private readonly CommentaireDAO _dao = new();
    private readonly EmployeDAO _employeDAO = new();
    private readonly int _idTache;

    private ListBox _liste = null!;
    private TextBox _txtCommentaire = null!;
    private ComboBox _cmbEmploye = null!;

    public FormCommentaires(int idTache, string titreTache)
    {
        _idTache = idTache;
        Text = $"Commentaires - {titreTache}";
        Width = 550;
        Height = 500;
        StartPosition = FormStartPosition.CenterParent;
        ConstruireUI();
        Charger();
    }

    private void ConstruireUI()
    {
        _liste = new ListBox { Dock = DockStyle.Fill };

        var panelBas = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 130,
            ColumnCount = 2,
            Padding = new Padding(10)
        };
        panelBas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        panelBas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

        _txtCommentaire = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
        _cmbEmploye = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Nom",
            ValueMember = "IdEmploye"
        };
        _cmbEmploye.Format += (s, e) => { if (e.ListItem is Employe emp) e.Value = emp.ToString(); };
        _cmbEmploye.DataSource = _employeDAO.GetAll();

        var btnAjouter = new Button { Text = "Ajouter le commentaire", Dock = DockStyle.Bottom, Height = 32 };
        btnAjouter.Click += (s, e) => Ajouter();

        panelBas.Controls.Add(new Label { Text = "Auteur", Dock = DockStyle.Top }, 1, 0);
        panelBas.Controls.Add(_cmbEmploye, 1, 0);
        panelBas.Controls.Add(new Label { Text = "Commentaire", Dock = DockStyle.Top }, 0, 0);
        panelBas.Controls.Add(_txtCommentaire, 0, 0);
        panelBas.SetRowSpan(_txtCommentaire, 1);

        Controls.Add(_liste);
        Controls.Add(btnAjouter);
        Controls.Add(panelBas);
    }

    private void Charger()
    {
        var commentaires = _dao.GetByTache(_idTache);
        _liste.Items.Clear();
        foreach (var c in commentaires)
            _liste.Items.Add($"[{c.DateCommentaire:dd/MM/yyyy}] {c.NomEmploye} : {c.Texte}");
    }

    private void Ajouter()
    {
        if (string.IsNullOrWhiteSpace(_txtCommentaire.Text))
        {
            MessageBox.Show("Le commentaire est vide.");
            return;
        }
        if (_cmbEmploye.SelectedValue == null)
        {
            MessageBox.Show("Selectionne un auteur.");
            return;
        }

        _dao.Insert(new Commentaire
        {
            Texte = _txtCommentaire.Text,
            DateCommentaire = DateTime.Today,
            IdEmploye = (int)_cmbEmploye.SelectedValue!,
            IdTache = _idTache
        });

        _txtCommentaire.Text = "";
        Charger();
    }
}

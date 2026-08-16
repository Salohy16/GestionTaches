using GestionTaches.Data;
using GestionTaches.Models;

namespace GestionTaches.Forms;

public class FormNotifications : Form
{
    private readonly NotificationDAO _dao = new();
    private readonly EmployeDAO _employeDAO = new();
    private ComboBox _cmbEmploye = null!;
    private ListBox _liste = null!;
    private List<Notification> _notificationsEnMemoire = new();

    public FormNotifications()
    {
        Text = "Notifications";
        Width = 600;
        Height = 500;
        StartPosition = FormStartPosition.CenterParent;
        ConstruireUI();
    }

    private void ConstruireUI()
    {
        _cmbEmploye = new ComboBox
        {
            Dock = DockStyle.Top,
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "Nom",
            ValueMember = "IdEmploye",
            Margin = new Padding(10)
        };
        _cmbEmploye.Format += (s, e) => { if (e.ListItem is Employe emp) e.Value = emp.ToString(); };
        _cmbEmploye.DataSource = _employeDAO.GetAll();
        _cmbEmploye.SelectedIndexChanged += (s, e) => Charger();

        var btnMarquerLue = new Button { Text = "Marquer comme lue", Dock = DockStyle.Bottom, Height = 32 };
        btnMarquerLue.Click += (s, e) => MarquerCommeLue();

        _liste = new ListBox { Dock = DockStyle.Fill };

        Controls.Add(_liste);
        Controls.Add(btnMarquerLue);
        Controls.Add(_cmbEmploye);

        Charger();
    }

    private void Charger()
    {
        if (_cmbEmploye.SelectedValue == null) return;
        int idEmploye = (int)_cmbEmploye.SelectedValue!;
        _notificationsEnMemoire = _dao.GetByEmploye(idEmploye);

        _liste.Items.Clear();
        foreach (var n in _notificationsEnMemoire)
        {
            string statut = n.Lu ? "" : "[NON LUE] ";
            _liste.Items.Add($"{statut}{n.DateNotif:dd/MM/yyyy} - {n.Message}");
        }

        if (_liste.Items.Count == 0)
            _liste.Items.Add("Aucune notification.");
    }

    private void MarquerCommeLue()
    {
        int index = _liste.SelectedIndex;
        if (index < 0 || index >= _notificationsEnMemoire.Count)
        {
            MessageBox.Show("Selectionne une notification.");
            return;
        }
        _dao.MarquerCommeLue(_notificationsEnMemoire[index].IdNotif);
        Charger();
    }
}

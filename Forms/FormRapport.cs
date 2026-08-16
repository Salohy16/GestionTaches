using GestionTaches.Data;

namespace GestionTaches.Forms;

public class FormRapport : Form
{
    private readonly TacheDAO _tacheDAO = new();
    private DataGridView _grid = null!;

    public FormRapport()
    {
        Text = "Rapport de productivite";
        Width = 750;
        Height = 500;
        StartPosition = FormStartPosition.CenterParent;
        ConstruireUI();
        Charger();
    }

    private void ConstruireUI()
    {
        var lbl = new Label
        {
            Text = "Productivite par employe",
            Dock = DockStyle.Top,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Height = 35,
            Padding = new Padding(10, 8, 0, 0)
        };

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        Controls.Add(_grid);
        Controls.Add(lbl);
    }

    private void Charger()
    {
        var data = _tacheDAO.RapportParEmploye();

        _grid.DataSource = data.Select(x => new
        {
            Employe = x.employe.ToString(),
            TachesTotal = x.total,
            Terminees = x.terminees,
            EnRetard = x.enRetard,
            TauxCompletion = x.total == 0 ? "-" : $"{(x.terminees * 100.0 / x.total):0.#} %"
        }).ToList();
    }
}

using GestionTaches.Data;

namespace GestionTaches.Forms;

public class FormHistorique : Form
{
    private readonly HistoriqueDAO _dao = new();
    private readonly TacheDAO _tacheDAO = new();
    private readonly int? _idTache;
    private ListBox _liste = null!;

    // Sans parametre : affiche l'historique de TOUTES les taches
    public FormHistorique()
    {
        _idTache = null;
        Text = "Historique de toutes les taches";
        Initialiser();
    }

    // Avec parametre : affiche l'historique d'une tache precise
    public FormHistorique(int idTache)
    {
        _idTache = idTache;
        var tache = _tacheDAO.GetAll().FirstOrDefault(t => t.IdTache == idTache);
        Text = $"Historique - {tache?.Titre ?? "Tache"}";
        Initialiser();
    }

    private void Initialiser()
    {
        Width = 600;
        Height = 500;
        StartPosition = FormStartPosition.CenterParent;

        _liste = new ListBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 9) };
        Controls.Add(_liste);

        Charger();
    }

    private void Charger()
    {
        var historique = _idTache.HasValue
            ? _dao.GetByTache(_idTache.Value)
            : _dao.GetAll();

        _liste.Items.Clear();
        foreach (var h in historique)
        {
            string prefixe = _idTache.HasValue ? "" : $"[Tache #{h.IdTache}] ";
            _liste.Items.Add($"{h.DateHistorique:dd/MM/yyyy} - {prefixe}{h.Action}");
        }

        if (_liste.Items.Count == 0)
            _liste.Items.Add("Aucun historique pour le moment.");
    }
}

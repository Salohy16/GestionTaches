namespace GestionTaches.Models;

public class Historique
{
    public int IdHistorique { get; set; }
    public string Action { get; set; } = "";
    public DateTime DateHistorique { get; set; }
    public int IdTache { get; set; }
}

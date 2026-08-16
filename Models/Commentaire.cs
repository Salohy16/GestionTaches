namespace GestionTaches.Models;

public class Commentaire
{
    public int IdComment { get; set; }
    public string Texte { get; set; } = "";
    public DateTime DateCommentaire { get; set; }
    public int IdEmploye { get; set; }
    public int IdTache { get; set; }

    // Champ de confort pour l'affichage
    public string NomEmploye { get; set; } = "";
}

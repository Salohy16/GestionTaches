namespace GestionTaches.Models;

public class Tache
{
    public int IdTache { get; set; }
    public int IdEmploye { get; set; }
    public string Titre { get; set; } = "";
    public string Description { get; set; } = "";
    public int Priorite { get; set; } // 1 = Basse, 2 = Moyenne, 3 = Haute
    public DateTime DateCreation { get; set; }
    public DateTime DateLimite { get; set; }
    public string Statut { get; set; } = "A faire"; // A faire / En cours / Terminee

    // Champs de confort pour l'affichage (jointure), non stockes en BD
    public string NomEmploye { get; set; } = "";

    public static string PrioriteLibelle(int p) => p switch
    {
        1 => "Basse",
        2 => "Moyenne",
        3 => "Haute",
        _ => "Inconnue"
    };

    public bool EstEnRetard => Statut != "Terminee" && DateLimite.Date < DateTime.Today;
}

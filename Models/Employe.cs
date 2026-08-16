namespace GestionTaches.Models;

public class Employe
{
    public int IdEmploye { get; set; }
    public string Nom { get; set; } = "";
    public string Prenom { get; set; } = "";
    public string Mail { get; set; } = "";

    public override string ToString() => $"{Prenom} {Nom}";
}

using GestionTaches.Models;
using MySql.Data.MySqlClient;

namespace GestionTaches.Data;

public class TacheDAO
{
    private readonly HistoriqueDAO _historiqueDAO = new();
    private readonly NotificationDAO _notificationDAO = new();

    private const string SelectBase = @"
        SELECT t.id_tache, t.idemploye, t.description, t.priorite, t.titre,
               t.dateCreation, t.dateLimite, t.statut,
               CONCAT(e.prenom_employe, ' ', e.nom_employe) AS nom_employe
        FROM tache t
        JOIN employe e ON e.idemploye = t.idemploye";

    public List<Tache> GetAll()
    {
        var liste = new List<Tache>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(SelectBase + " ORDER BY t.dateLimite", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            liste.Add(Lire(reader));
        return liste;
    }

    public List<Tache> GetByEmploye(int idEmploye)
    {
        var liste = new List<Tache>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(SelectBase + " WHERE t.idemploye = @id ORDER BY t.dateLimite", conn);
        cmd.Parameters.AddWithValue("@id", idEmploye);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            liste.Add(Lire(reader));
        return liste;
    }

    private static Tache Lire(MySqlDataReader reader) => new()
    {
        IdTache = reader.GetInt32("id_tache"),
        IdEmploye = reader.GetInt32("idemploye"),
        Description = reader.GetString("description"),
        Priorite = reader.GetInt32("priorite"),
        Titre = reader.GetString("titre"),
        DateCreation = reader.GetDateTime("dateCreation"),
        DateLimite = reader.GetDateTime("dateLimite"),
        Statut = reader.GetString("statut"),
        NomEmploye = reader.GetString("nom_employe")
    };

    public int Insert(Tache t)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(@"
            INSERT INTO tache (idemploye, description, priorite, titre, dateCreation, dateLimite, statut)
            VALUES (@idemploye, @description, @priorite, @titre, @dateCreation, @dateLimite, @statut);
            SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@idemploye", t.IdEmploye);
        cmd.Parameters.AddWithValue("@description", t.Description);
        cmd.Parameters.AddWithValue("@priorite", t.Priorite);
        cmd.Parameters.AddWithValue("@titre", t.Titre);
        cmd.Parameters.AddWithValue("@dateCreation", t.DateCreation);
        cmd.Parameters.AddWithValue("@dateLimite", t.DateLimite);
        cmd.Parameters.AddWithValue("@statut", t.Statut);
        int id = Convert.ToInt32(cmd.ExecuteScalar());

        _historiqueDAO.Ajouter(id, $"Creation de la tache \"{t.Titre}\"");
        _notificationDAO.Ajouter(t.IdEmploye, $"Nouvelle tache assignee : \"{t.Titre}\"");

        return id;
    }

    public void Update(Tache t, int? ancienEmploye = null, string? ancienStatut = null)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(@"
            UPDATE tache SET idemploye = @idemploye, description = @description, priorite = @priorite,
                   titre = @titre, dateLimite = @dateLimite, statut = @statut
            WHERE id_tache = @id", conn);
        cmd.Parameters.AddWithValue("@idemploye", t.IdEmploye);
        cmd.Parameters.AddWithValue("@description", t.Description);
        cmd.Parameters.AddWithValue("@priorite", t.Priorite);
        cmd.Parameters.AddWithValue("@titre", t.Titre);
        cmd.Parameters.AddWithValue("@dateLimite", t.DateLimite);
        cmd.Parameters.AddWithValue("@statut", t.Statut);
        cmd.Parameters.AddWithValue("@id", t.IdTache);
        cmd.ExecuteNonQuery();

        _historiqueDAO.Ajouter(t.IdTache, $"Modification de la tache \"{t.Titre}\"");

        if (ancienStatut != null && ancienStatut != t.Statut)
        {
            _historiqueDAO.Ajouter(t.IdTache, $"Statut change : {ancienStatut} -> {t.Statut}");
            _notificationDAO.Ajouter(t.IdEmploye, $"La tache \"{t.Titre}\" est passee au statut \"{t.Statut}\"");
        }

        if (ancienEmploye != null && ancienEmploye != t.IdEmploye)
        {
            _notificationDAO.Ajouter(t.IdEmploye, $"La tache \"{t.Titre}\" vous a ete attribuee");
        }
    }

    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand("DELETE FROM tache WHERE id_tache = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    // Statistiques pour le rapport de productivite
    public List<(Employe employe, int total, int terminees, int enRetard)> RapportParEmploye()
    {
        var resultat = new List<(Employe, int, int, int)>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(@"
            SELECT e.idemploye, e.nom_employe, e.prenom_employe, e.mail_employe,
                   COUNT(t.id_tache) AS total,
                   SUM(CASE WHEN t.statut = 'Terminee' THEN 1 ELSE 0 END) AS terminees,
                   SUM(CASE WHEN t.statut <> 'Terminee' AND t.dateLimite < CURDATE() THEN 1 ELSE 0 END) AS enRetard
            FROM employe e
            LEFT JOIN tache t ON t.idemploye = e.idemploye
            GROUP BY e.idemploye, e.nom_employe, e.prenom_employe, e.mail_employe
            ORDER BY e.nom_employe", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var emp = new Employe
            {
                IdEmploye = reader.GetInt32("idemploye"),
                Nom = reader.GetString("nom_employe"),
                Prenom = reader.GetString("prenom_employe"),
                Mail = reader.GetString("mail_employe")
            };
            int total = reader.GetInt32("total");
            int terminees = reader.IsDBNull(reader.GetOrdinal("terminees")) ? 0 : Convert.ToInt32(reader["terminees"]);
            int enRetard = reader.IsDBNull(reader.GetOrdinal("enRetard")) ? 0 : Convert.ToInt32(reader["enRetard"]);
            resultat.Add((emp, total, terminees, enRetard));
        }
        return resultat;
    }
}

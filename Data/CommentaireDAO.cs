using GestionTaches.Models;
using MySql.Data.MySqlClient;

namespace GestionTaches.Data;

public class CommentaireDAO
{
    private readonly HistoriqueDAO _historiqueDAO = new();

    public List<Commentaire> GetByTache(int idTache)
    {
        var liste = new List<Commentaire>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(@"
            SELECT c.id_comment, c.texte, c.date_commentaire, c.employe, c.id_tache,
                   CONCAT(e.prenom_employe, ' ', e.nom_employe) AS nom_employe
            FROM commentaire c
            JOIN employe e ON e.idemploye = c.employe
            WHERE c.id_tache = @id
            ORDER BY c.date_commentaire", conn);
        cmd.Parameters.AddWithValue("@id", idTache);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            liste.Add(new Commentaire
            {
                IdComment = reader.GetInt32("id_comment"),
                Texte = reader.GetString("texte"),
                DateCommentaire = reader.GetDateTime("date_commentaire"),
                IdEmploye = reader.GetInt32("employe"),
                IdTache = reader.GetInt32("id_tache"),
                NomEmploye = reader.GetString("nom_employe")
            });
        }
        return liste;
    }

    public void Insert(Commentaire c)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(@"
            INSERT INTO commentaire (texte, date_commentaire, employe, id_tache)
            VALUES (@texte, @date, @employe, @idTache)", conn);
        cmd.Parameters.AddWithValue("@texte", c.Texte);
        cmd.Parameters.AddWithValue("@date", c.DateCommentaire);
        cmd.Parameters.AddWithValue("@employe", c.IdEmploye);
        cmd.Parameters.AddWithValue("@idTache", c.IdTache);
        cmd.ExecuteNonQuery();

        _historiqueDAO.Ajouter(c.IdTache, "Nouveau commentaire ajoute");
    }

    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand("DELETE FROM commentaire WHERE id_comment = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}

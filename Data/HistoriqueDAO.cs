using GestionTaches.Models;
using MySql.Data.MySqlClient;

namespace GestionTaches.Data;

public class HistoriqueDAO
{
    public void Ajouter(int idTache, string action)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(
            "INSERT INTO historique (action, date_historique, id_tache) VALUES (@action, @date, @idTache)", conn);
        cmd.Parameters.AddWithValue("@action", action);
        cmd.Parameters.AddWithValue("@date", DateTime.Today);
        cmd.Parameters.AddWithValue("@idTache", idTache);
        cmd.ExecuteNonQuery();
    }

    public List<Historique> GetByTache(int idTache)
    {
        var liste = new List<Historique>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id_historique, action, date_historique, id_tache FROM historique WHERE id_tache = @id ORDER BY date_historique DESC, id_historique DESC", conn);
        cmd.Parameters.AddWithValue("@id", idTache);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            liste.Add(new Historique
            {
                IdHistorique = reader.GetInt32("id_historique"),
                Action = reader.GetString("action"),
                DateHistorique = reader.GetDateTime("date_historique"),
                IdTache = reader.GetInt32("id_tache")
            });
        }
        return liste;
    }

    public List<Historique> GetAll()
    {
        var liste = new List<Historique>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id_historique, action, date_historique, id_tache FROM historique ORDER BY date_historique DESC, id_historique DESC", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            liste.Add(new Historique
            {
                IdHistorique = reader.GetInt32("id_historique"),
                Action = reader.GetString("action"),
                DateHistorique = reader.GetDateTime("date_historique"),
                IdTache = reader.GetInt32("id_tache")
            });
        }
        return liste;
    }
}

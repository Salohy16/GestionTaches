using GestionTaches.Models;
using MySql.Data.MySqlClient;

namespace GestionTaches.Data;

public class NotificationDAO
{
    public void Ajouter(int idEmploye, string message)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(
            "INSERT INTO notification (message, date_notif, lu, idemploye) VALUES (@message, @date, 0, @idEmploye)", conn);
        cmd.Parameters.AddWithValue("@message", message);
        cmd.Parameters.AddWithValue("@date", DateTime.Today);
        cmd.Parameters.AddWithValue("@idEmploye", idEmploye);
        cmd.ExecuteNonQuery();
    }

    public List<Notification> GetByEmploye(int idEmploye)
    {
        var liste = new List<Notification>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(
            "SELECT id_notif, message, date_notif, lu, idemploye FROM notification WHERE idemploye = @id ORDER BY date_notif DESC, id_notif DESC", conn);
        cmd.Parameters.AddWithValue("@id", idEmploye);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            liste.Add(new Notification
            {
                IdNotif = reader.GetInt32("id_notif"),
                Message = reader.GetString("message"),
                DateNotif = reader.GetDateTime("date_notif"),
                Lu = reader.GetBoolean("lu"),
                IdEmploye = reader.GetInt32("idemploye")
            });
        }
        return liste;
    }

    public void MarquerCommeLue(int idNotif)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand("UPDATE notification SET lu = 1 WHERE id_notif = @id", conn);
        cmd.Parameters.AddWithValue("@id", idNotif);
        cmd.ExecuteNonQuery();
    }
}

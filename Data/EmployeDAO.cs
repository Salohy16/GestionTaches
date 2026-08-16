using GestionTaches.Models;
using MySql.Data.MySqlClient;

namespace GestionTaches.Data;

public class EmployeDAO
{
    public List<Employe> GetAll()
    {
        var liste = new List<Employe>();
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand("SELECT idemploye, nom_employe, prenom_employe, mail_employe FROM employe ORDER BY nom_employe", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            liste.Add(new Employe
            {
                IdEmploye = reader.GetInt32("idemploye"),
                Nom = reader.GetString("nom_employe"),
                Prenom = reader.GetString("prenom_employe"),
                Mail = reader.GetString("mail_employe")
            });
        }
        return liste;
    }

    public Employe? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand("SELECT idemploye, nom_employe, prenom_employe, mail_employe FROM employe WHERE idemploye = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Employe
            {
                IdEmploye = reader.GetInt32("idemploye"),
                Nom = reader.GetString("nom_employe"),
                Prenom = reader.GetString("prenom_employe"),
                Mail = reader.GetString("mail_employe")
            };
        }
        return null;
    }

    public int Insert(Employe e)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(
            "INSERT INTO employe (nom_employe, prenom_employe, mail_employe) VALUES (@nom, @prenom, @mail); SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@nom", e.Nom);
        cmd.Parameters.AddWithValue("@prenom", e.Prenom);
        cmd.Parameters.AddWithValue("@mail", e.Mail);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Employe e)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand(
            "UPDATE employe SET nom_employe = @nom, prenom_employe = @prenom, mail_employe = @mail WHERE idemploye = @id", conn);
        cmd.Parameters.AddWithValue("@nom", e.Nom);
        cmd.Parameters.AddWithValue("@prenom", e.Prenom);
        cmd.Parameters.AddWithValue("@mail", e.Mail);
        cmd.Parameters.AddWithValue("@id", e.IdEmploye);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new MySqlCommand("DELETE FROM employe WHERE idemploye = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
}

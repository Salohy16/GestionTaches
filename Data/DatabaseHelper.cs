using MySql.Data.MySqlClient;

namespace GestionTaches.Data;

public static class DatabaseHelper
{
    // Adapte ces valeurs a ta configuration MySQL / phpMyAdmin locale.
    private const string Server = "127.0.0.1";
    private const string Port = "3306";
    private const string Database = "bd_tache";
    private const string User = "root";
    private const string Password = ""; // mets ton mot de passe MySQL ici si besoin

    private static string ConnectionString =>
        $"Server={Server};Port={Port};Database={Database};Uid={User};Pwd={Password};";

    public static MySqlConnection GetConnection()
    {
        var conn = new MySqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }

    public static bool TestConnection(out string erreur)
    {
        try
        {
            using var conn = GetConnection();
            erreur = "";
            return true;
        }
        catch (Exception ex)
        {
            erreur = ex.Message;
            return false;
        }
    }
}

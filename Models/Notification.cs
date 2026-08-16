namespace GestionTaches.Models;

public class Notification
{
    public int IdNotif { get; set; }
    public string Message { get; set; } = "";
    public DateTime DateNotif { get; set; }
    public bool Lu { get; set; }
    public int IdEmploye { get; set; }
}

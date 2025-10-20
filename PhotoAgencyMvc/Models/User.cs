namespace PhotoAgencyMvc.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;  

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual Role Role { get; set; }

    public virtual Client Client { get; set; }  

    public virtual Photographer Photographer { get; set; }  
}
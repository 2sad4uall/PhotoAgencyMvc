using Microsoft.AspNetCore.Identity;

namespace PhotoAgencyMvc.Models;

public class User : IdentityUser<int>  
{
    public int RoleId { get; set; }  

    public virtual Client? Client { get; set; }

    public virtual Photographer? Photographer { get; set; }

    public virtual Role Role { get; set; } = null!;
}

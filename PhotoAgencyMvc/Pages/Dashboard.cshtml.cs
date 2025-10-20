using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

[Authorize]
public class DashboardModel : PageModel
{
    public void OnGet()
    {
        
    }
}
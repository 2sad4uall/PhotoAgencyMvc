using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Photographer")]
public class PhotographerDashboardModel : PageModel
{
    public void OnGet()
    {
    }
}
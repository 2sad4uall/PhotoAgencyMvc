using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoAgencyMvc.Models;
using Microsoft.EntityFrameworkCore;
public class ServicesModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public ServicesModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Service> Services { get; set; } = new List<Service>();

    public async Task OnGetAsync()
    {
        Services = await _context.Services.ToListAsync();
    }
}
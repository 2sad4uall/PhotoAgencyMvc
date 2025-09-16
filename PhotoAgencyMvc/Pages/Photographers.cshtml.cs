using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoAgencyMvc.Models;
using Microsoft.EntityFrameworkCore;
public class PhotographersModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public PhotographersModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Photographer> Photographers { get; set; } = new List<Photographer>();

    public async Task OnGetAsync()
    {
        Photographers = await _context.Photographers.ToListAsync();
    }
}
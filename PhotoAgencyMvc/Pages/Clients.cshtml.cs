using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;

[Authorize(Roles = "Admin")]
public class ClientsModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public ClientsModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Client> Clients { get; set; } = new List<Client>();

    public async Task OnGetAsync()
    {
        Clients = await _context.Clients.ToListAsync();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoAgencyMvc.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Client")]
public class ClientOrdersModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public ClientOrdersModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Order> Orders { get; set; } = new List<Order>();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == int.Parse(userId));
        if (client != null)
        {
            Orders = await _context.Orders.Where(o => o.ClientId == client.Id).ToListAsync();
        }
    }
}
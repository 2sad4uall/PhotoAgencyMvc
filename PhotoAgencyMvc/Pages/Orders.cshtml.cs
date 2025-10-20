using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Security.Claims;

[Authorize]
public class OrdersModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public OrdersModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Order> Orders { get; set; } = new List<Order>();
    
    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var photographer =  await _context.Photographers.FirstOrDefaultAsync(p => p.UserId.ToString() == userId); 
        if (User.IsInRole("Client") || User.IsInRole("Admin") )
            Orders = await _context.Orders.Include(o => o.Client).Include(o => o.Service).ToListAsync();
        else
            Orders = await _context.Orders.Include(o => o.Client).Include(o => o.Service).Where(o => o.Service.PhotographerId == photographer.Id).ToListAsync();
    }
}
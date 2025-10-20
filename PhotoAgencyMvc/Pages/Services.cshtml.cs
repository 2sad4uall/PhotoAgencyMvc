using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Security.Claims;

public class ServicesModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public ServicesModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Service> Services { get; set; } = new List<Service>();

    public string CurrentFilter { get; set; }
    public string NameSortParm { get; set; }
    public string PriceSortParm { get; set; }
    public Photographer Photographer { get; set; }
    public async Task OnGetAsync(string searchString, string sortOrder)
    {
        CurrentFilter = searchString;
        NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        PriceSortParm = sortOrder == "price" ? "price_desc" : "price";
        Services = await _context.Services
            .Include(s => s.Photographer)
            .ToListAsync();
        var services = _context.Services.Include(s => s.Photographer).AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            services = services.Where(s => s.Name.Contains(searchString));
        }

        switch (sortOrder)
        {
            case "name_desc":
                services = services.OrderByDescending(s => s.Name);
                break;
            case "price":
                services = services.OrderBy(s => s.Price);
                break;
            case "price_desc":
                services = services.OrderByDescending(s => s.Price);
                break;
            default:
                services = services.OrderBy(s => s.Name);
                break;
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var photographer = await _context.Photographers.FirstOrDefaultAsync(p => p.UserId.ToString() == userId);
        if (User.IsInRole("Photographer"))
            Services = await services.Where(o => o.PhotographerId == photographer.Id).ToListAsync();
        else
            Services = await services.ToListAsync();
    }
}
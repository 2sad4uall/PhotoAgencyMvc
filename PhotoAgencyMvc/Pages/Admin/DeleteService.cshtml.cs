using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "Photographer, Admin")]
public class DeleteServiceModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public DeleteServiceModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Service Service { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);
        var admin = await _context.Users.FirstOrDefaultAsync(p => p.Id.ToString() == userId);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        if (!User.IsInRole("Admin"))
        {
            if (photographer == null)
            {
                return NotFound("Фотограф не найден.");
            }

            Service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.PhotographerId == photographer.Id);
            if (Service == null)
            {
                return NotFound("Услуга не найдена или не принадлежит вам.");
            }
        }
        else
        {
            if (admin == null)
            {
                return NotFound("Админ не найден.");
            }

            Service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);
            if (Service == null)
            {
                return NotFound("Услуга не найдена или не принадлежит вам.");
            }
        }
        

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);
        var admin = await _context.Users.FirstOrDefaultAsync(p => p.Id.ToString() == userId);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        if (!User.IsInRole("Admin"))
        {
            if (photographer == null)
            {
                return NotFound("Фотограф не найден.");
            }

            Service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.PhotographerId == photographer.Id);
            if (Service == null)
            {
                return NotFound("Услуга не найдена или не принадлежит вам.");
            }
        }
        else
        {
            if (admin == null)
            {
                return NotFound("Админ не найден.");
            }

            Service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);
            if (Service == null)
            {
                return NotFound("Услуга не найдена или не принадлежит вам.");
            }
        }

        _context.Services.Remove(Service);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Services");
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "Photographer")] // Ограничение доступа только для фотографов
public class CreateServiceModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public CreateServiceModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Service Service { get; set; }

    public IActionResult OnGet()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Инициализация Service с PhotographerId текущего фотографа
        var photographer = _context.Photographers
            .FirstOrDefault(p => p.UserId.ToString() == userId);
        if (photographer == null)
        {
            return NotFound("Фотограф не найден.");
        }

        Service = new Service
        {
            PhotographerId = photographer.Id
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);
        if (photographer == null)
        {
            return NotFound("Фотограф не найден.");
        }

        Service.PhotographerId = photographer.Id; // Убедимся, что PhotographerId установлен

        _context.Services.Add(Service);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Services");
    }
}
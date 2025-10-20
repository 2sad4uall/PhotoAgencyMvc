using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]

public class ProfileModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public ProfileModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Client ClientProfile { get; set; }
    [BindProperty]
    public Photographer PhotographerProfile { get; set; }
    public string UserRole { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        UserRole = User.IsInRole("Photographer") ? "Photographer" : "Client";

        if (UserRole == "Client")
        {
            ClientProfile = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);
            if (ClientProfile == null)
            {
                return NotFound("Клиент не найден.");
            }
        }
        else if (UserRole == "Photographer")
        {
            PhotographerProfile = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);
            if (PhotographerProfile == null)
            {
                return NotFound("Фотограф не найден.");
            }
        }
        else
        {
            return Unauthorized();
        } 


        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        if (User.IsInRole("Client"))
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId.ToString() == userId);
            if (client == null)
            {
                return NotFound("Клиент не найден.");
            }

            client.FullName = ClientProfile.FullName;
            client.Phone = ClientProfile.Phone;
            client.Address = ClientProfile.Address;
        }
        else
        {
            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.UserId.ToString() == userId);
            if (photographer == null)
            {
                return NotFound("Фотограф не найден.");
            }

            photographer.FullName = PhotographerProfile.FullName;
            photographer.Phone = PhotographerProfile.Phone;
            photographer.Bio = PhotographerProfile.Bio;
        }

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToPage("/Profile");
        }
        catch (DbUpdateException ex)
        {
            ModelState.AddModelError("", $"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}");
            return Page();
        }
    }
}
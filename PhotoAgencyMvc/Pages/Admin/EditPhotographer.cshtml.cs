using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class EditPhotographerModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public EditPhotographerModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Photographer Photographer { get; set; }

    [BindProperty]
    public IFormFile PhotoFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Photographer = await _context.Photographers.FindAsync(id);
        if (Photographer == null)
        {
            return NotFound();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var photographerToUpdate = await _context.Photographers.FindAsync(Photographer.Id);
        if (photographerToUpdate == null)
        {
            return NotFound();
        }
        photographerToUpdate.UserId = photographerToUpdate.UserId;
        photographerToUpdate.FullName = Photographer.FullName;
        photographerToUpdate.Phone = Photographer.Phone;
        photographerToUpdate.Bio = Photographer.Bio;

        // Обработка нового фото, если оно загружено
        if (PhotoFile != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await PhotoFile.CopyToAsync(memoryStream);
                photographerToUpdate.Photo = memoryStream.ToArray(); 
            }
        }

        try
        {
            _context.Photographers.Update(photographerToUpdate);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Photographers");
        }
        catch (DbUpdateException ex)
        {
            ModelState.AddModelError(string.Empty, $"Ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}");
            return Page();
        }
    }
}
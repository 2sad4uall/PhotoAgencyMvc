using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;

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

        if (PhotoFile != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await PhotoFile.CopyToAsync(memoryStream);
                Photographer.Photo = Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        _context.Attach(Photographer).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToPage("/Photographers");
    }
}
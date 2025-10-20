using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class DeletePhotographerModel : PageModel
{
    private readonly PhotoAgencyContext _context;
    

    public DeletePhotographerModel(PhotoAgencyContext context)
    {
        _context = context;
    
    }

    [BindProperty]
    public Photographer Photographer { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.Id == id);
        if (Photographer == null)
        {
            return NotFound("Фотограф не найден.");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var photographer = await _context.Photographers
            .FirstOrDefaultAsync(p => p.Id == id);
        if (photographer == null)
        {
            return NotFound("Фотограф не найден.");
        }

        

        _context.Photographers.Remove(photographer);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Photographers");
    }
}
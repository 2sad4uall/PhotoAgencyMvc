using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;

[Authorize(Roles = "Admin")]
public class EditServiceModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public EditServiceModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Service Service { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Service = await _context.Services.FindAsync(id);
        if (Service == null)
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

        _context.Attach(Service).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToPage("/Services");
    }
}
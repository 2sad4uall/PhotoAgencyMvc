using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;

[Authorize(Roles = "Admin")]
public class EditOrderModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public EditOrderModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Order Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Order = await _context.Orders.FindAsync(id);
        if (Order == null)
        {
            return NotFound();
        }
        ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName");
        ViewData["PhotographerId"] = new SelectList(_context.Photographers, "Id", "FullName");
        ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToPage("/Orders");
    }
}
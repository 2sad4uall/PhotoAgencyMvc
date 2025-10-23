using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class CreatePaymentModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public CreatePaymentModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Payment Payment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ViewData["OrderId"] = new SelectList(await _context.Orders.ToListAsync(), "Id", "Id");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["OrderId"] = new SelectList(await _context.Orders.ToListAsync(), "Id", "Id", Payment.OrderId);
            return Page();
        }

        Payment.PaymentDate = DateTime.UtcNow; 
        _context.Payments.Add(Payment);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Payments");
    }
}
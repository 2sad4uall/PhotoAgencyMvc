using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoAgencyMvc.Models;
using Microsoft.EntityFrameworkCore;
public class PaymentsModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public PaymentsModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Payment> Payments { get; set; } = new List<Payment>();

    public async Task OnGetAsync()
    {
        Payments = await _context.Payments.ToListAsync();
    }
}
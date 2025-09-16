using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoAgencyMvc.Models;
using Microsoft.EntityFrameworkCore;
public class ReviewsModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public ReviewsModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    public IList<Review> Reviews { get; set; } = new List<Review>();

    public async Task OnGetAsync()
    {
        Reviews = await _context.Reviews.ToListAsync();
    }
}
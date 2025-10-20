using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System.Security.Claims;
[Authorize(Roles = "Client")]
public class CreateModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public CreateModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Order Order { get; set; }
    public string ClientFullName { get; set; }
    public string ServiceName { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId.ToString() == userId);
        if (client == null)
        {
            return NotFound("������ �� ������.");
        }

        var service = await _context.Services.FindAsync(id);
        if (service == null)
        {
            return NotFound("������ �� �������.");
        }

        
        Order = new Order
        {
            ClientId = client.Id,
            ServiceId = id,
            OrderDate = DateTime.Now,
            Status = "�����"
        };
        ViewData["ClientFullName"] = client.FullName;
        ViewData["ServiceName"] = service.Name;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        
        if (Order == null)
        {
            ModelState.AddModelError("", "������ Order �� ��������.");
            return Page();
        }

        
        if (id != Order.ServiceId)
        {
            ModelState.AddModelError("", "ID ������ � ������� �� ��������� � ID � ������.");
            return Page();
        }

        var client = await _context.Clients.FindAsync(Order.ClientId);
        var service = await _context.Services.FindAsync(Order.ServiceId);
        if (client == null)
        {
            ModelState.AddModelError("Order.ClientId", "������ �� ������.");
            return Page();
        }
        if (service == null)
        {
            ModelState.AddModelError("Order.ServiceId", "������ �� �������.");
            return Page();
        }
        ModelState.ClearValidationState("Client");
        ModelState.ClearValidationState("Service");
        ModelState.Clear();

        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine($"Validation Error: {error.ErrorMessage}");
            }
            return Page();
        }

        if (await _context.Orders.AnyAsync(o => o.ClientId == Order.ClientId && o.ServiceId == Order.ServiceId && o.Status != "��������"))
        {
            ModelState.AddModelError("", "�� ��� �������� ��� ������.");
            return Page();
        }

        Order.OrderDate = DateTime.Now;
        Order.Status = "�����";
        try
        {
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Services");
        }
        catch (DbUpdateException ex)
        {
            ModelState.AddModelError("", $"������ ���������� � ��: {ex.InnerException?.Message ?? ex.Message}");
            return Page();
        }
    }
}
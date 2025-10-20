using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using PhotoAgencyMvc.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

public class LoginModel : PageModel
{
    private readonly PhotoAgencyContext _context;
    public LoginModel(PhotoAgencyContext context)
    {
        _context = context;
    }
    [BindProperty]
    public InputModel Input { get; set; }
    public string ReturnUrl { get; set; }
    public string Message { get; set; } 
    public class InputModel
    {
        [Required(ErrorMessage = "Логин обязателен")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public void OnGet(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == Input.Username);
            if (user != null && user.Password == Input.Password) 
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.Name )
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                switch (user.RoleId)
                {
                    case 1: 
                        return RedirectToPage("Admin/Dashboard", new { area = "Admin" });
                    case 2:
                        return RedirectToPage("/Dashboard", new { area = "Photographer" });
                    default:
                        return RedirectToPage("/Dashboard", new { area = "Client" });
                }
            }
            else
            {
                Message = "Неверный логин или пароль.";
                return Page();
            }
        }
        return Page();
    }
}
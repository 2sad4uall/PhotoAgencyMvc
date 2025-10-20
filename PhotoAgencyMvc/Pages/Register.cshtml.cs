using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhotoAgencyMvc.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class RegisterModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public RegisterModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } 
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [StringLength(100)]
        public string Address { get; set; }
    }
    public void OnGet()
    {
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            if (await _context.Users.AnyAsync(u => u.Username == Input.Username))
            {
                ModelState.AddModelError("Input.Username", "Логин уже используется");
                return Page();
            }
            if (await _context.Users.AnyAsync(u => u.Email == Input.Email))
            {
                ModelState.AddModelError("Input.Email", "Почта уже используется");
                return Page();
            }
            var user = new User
            {
                Username = Input.Username,
                Password = Input.Password, 
                Email = Input.Email,
                RoleId = 3 // Client по умолчанию
            };
            _context.Users.Add(user);            
            await _context.SaveChangesAsync();           
            var client = new Client
            {
                UserId = user.Id, 
                FullName = Input.FullName,
                Phone = Input.Phone,
                Address = Input.Address
            };
            _context.Clients.Add(client);            
            await _context.SaveChangesAsync();
            return RedirectToPage("/Login");
        }
        return Page();
    }
}
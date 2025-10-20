using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoAgencyMvc.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class CreatePhotographerModel : PageModel
{
    private readonly PhotoAgencyContext _context;

    public CreatePhotographerModel(PhotoAgencyContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [StringLength(500)]
        public string Bio { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public IFormFile Photo { get; set; }
    }

    public IActionResult OnGet()
    {
        
        

        Input = new InputModel();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        
        var user = new User
        {
            Username = Input.Username,
            Password = Input.Password,
            Email = Input.Email,
            RoleId = 2
        };
        _context.Users.Add(user);

        
        byte[] photoData = null;
        if (Input.Photo != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await Input.Photo.CopyToAsync(memoryStream);
                photoData = memoryStream.ToArray();
            }
        }

        
        var photographer = new Photographer
        {
            UserId = user.Id,
            FullName = Input.FullName,
            Phone = Input.Phone,
            Bio = Input.Bio,
            Photo = photoData
        };
        _context.Photographers.Add(photographer);

        await _context.SaveChangesAsync();

        return RedirectToPage("/Photographers");
    }


}

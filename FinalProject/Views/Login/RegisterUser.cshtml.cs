using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinalProject.Views.Login;

public class RegisterUser : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty]
    public string ConfirmPassword { get; set; }

    public string Message { get; set; }
    public bool IsSuccess { get; set; }

    public void OnGet()
    {
        // Initialize page
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || Password != ConfirmPassword)
        {
            Message = "Passwords do not match or form is invalid.";
            IsSuccess = false;
            return Page();
        }

        var user = new ApplicationUser { UserName = Username };
        var result = await _userManager.CreateAsync(user, Password);

        if (result.Succeeded)
        {
            // Ensure the "User" role exists
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");

            Message = "Registration successful!";
            IsSuccess = true;
        }
        else
        {
            Message = string.Join(", ", result.Errors.Select(e => e.Description));
            IsSuccess = false;
        }

        return Page();
    }
}
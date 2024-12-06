using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinalProject.Views.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public Login Login { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Display the page without errors on initial GET request
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Find the user by username
                var user = await _userManager.FindByNameAsync(Login.Username);
                if (user != null)
                {
                    // Sign-in the user
                    var result = await _signInManager.PasswordSignInAsync(user, Login.Password, false, false);
                    if (result.Succeeded)
                    {
                        // Redirect to the home page after successful login
                        return RedirectToPage("/Index");
                    }
                }

                // If we reached here, there was an error
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }

            return Page();
        }
    }
}
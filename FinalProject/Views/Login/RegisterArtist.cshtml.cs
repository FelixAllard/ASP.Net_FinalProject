using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinalProject.Views.Login;

public class RegisterArtist : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterArtist(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string ArtistName { get; set; }

        [BindProperty]
        public string Bio { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public void OnGet()
        {
            // Initialize the page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Password != ConfirmPassword)
            {
                Message = "Passwords do not match or form is invalid.";
                IsSuccess = false;
                return Page();
            }

            var user = new ApplicationUser 
            { 
                UserName = Username, 
                Email = Email, 
                ArtistName = ArtistName, 
                Bio = Bio 
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                // Assign the User and Artist roles
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Artist");

                Message = "Registration successful, Artist role assigned.";
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
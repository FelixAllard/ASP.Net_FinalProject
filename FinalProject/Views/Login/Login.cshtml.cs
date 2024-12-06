using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace FinalProject.Views.Login;

public class Login : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public Login(UserManager<ApplicationUser> userManager, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public void OnGet()
        {
            // Initialization logic if needed
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Message = "Please provide both username and password.";
                IsSuccess = false;
                return Page();
            }

            var user = await _userManager.FindByNameAsync(Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, Password))
            {
                // Create JWT token
                var roles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                        SecurityAlgorithms.HmacSha256
                    )
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Optionally, store the token in a session or as a cookie
                // HttpContext.Session.SetString("JWToken", tokenString);
                Message = "Login successful!";
                IsSuccess = true;
                return RedirectToPage("/Home/Index");  // Redirect to your desired page after login
            }

            Message = "Invalid login attempt.";
            IsSuccess = false;
            return Page();
        }
    }
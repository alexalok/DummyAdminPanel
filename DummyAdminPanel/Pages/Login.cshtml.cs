using DummyAdminPanel.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DummyAdminPanel.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = null!;

        [BindProperty]
        public string Password { get; set; } = null!;

        [BindProperty]
        public bool RememberMe { get; set; }

        readonly IAuthenticationService _authenticator;

        public LoginModel(IAuthenticationService authenticator)
        {
            _authenticator = authenticator;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var isAuthenticated = await _authenticator.TryAuthenticate(Email, Password, RememberMe);

            if (isAuthenticated)
                return LocalRedirect(returnUrl ?? "~/");

            ModelState.AddModelError("", "Invalid credentials.");
            return Page();
        }
    }
}

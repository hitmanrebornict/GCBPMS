using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace GCBPMS.Areas.Identity.Pages.Account
{
    public class LogoutPageModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LogoutPageModel(SignInManager<IdentityUser> signinManager)
        {
            _signInManager = signinManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Log out the user immediately
            await _signInManager.SignOutAsync();

            // Redirect to the desired page after logout (e.g., home page)
            return RedirectToPage("/Account/login"); // or RedirectToPage("/Account/Login") for login page
        }
    }
}

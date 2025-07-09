using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace TaskPlanner.Controllers
{
    public class CultureController : Controller
    {
        [HttpGet]
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            // Validate culture
            if (string.IsNullOrEmpty(culture) || (culture != "en" && culture != "uk"))
            {
                return RedirectToPage("/Index");
            }
            
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { 
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                }
            );

            return LocalRedirect(returnUrl ?? "/");
        }
    }
} 
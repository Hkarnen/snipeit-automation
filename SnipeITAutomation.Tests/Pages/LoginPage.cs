using System.Threading.Tasks;
using Microsoft.Playwright;

namespace SnipeITAutomation.Tests.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page) => _page = page;

        /// <summary>
        /// Navigates to the Snipe-IT demo login page.
        /// </summary>
        public async Task GoToAsync() =>
            await _page.GotoAsync("https://demo.snipeitapp.com/login");

        /// <summary>
        /// Logs in with the provided username and password.
        /// </summary>
        public async Task LoginAsync(string username, string password)
        {
            // Fill in login form
            await _page.FillAsync("input[name='username']", username);
            await _page.FillAsync("input[name='password']", password);

            // Click the login button
            await _page.ClickAsync("button[type='submit']");

            // Wait for dashboard to load
            await _page.WaitForURLAsync("**/dashboard");
        }
    }
}

using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace SnipeITAutomation.Tests
{
    public class AssetTests
    {
        [Test]
        public async Task LoginToSnipeITDemo()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            // Navigate to demo login page
            await page.GotoAsync("https://demo.snipeitapp.com/login");

            // Fill in login form (Username = "admin", Password = "password")
            await page.FillAsync("input[name='username']", "admin");
            await page.FillAsync("input[name='password']", "password");
            await page.ClickAsync("button[type='submit']");

            // Wait for dashboard
            await page.WaitForURLAsync("**/dashboard");

            Assert.That(page.Url, Does.Contain("dashboard"));

        }
    }
}

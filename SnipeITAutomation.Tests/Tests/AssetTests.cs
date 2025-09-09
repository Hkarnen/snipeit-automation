using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using SnipeITAutomation.Tests.Pages;

namespace SnipeITAutomation.Tests
{
    public class AssetTests
    {
        [Test]
        public async Task CanLoginToDashboard()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            var login = new LoginPage(page);
            await login.GoToAsync();
            await login.LoginAsync("admin", "password");

        }

        [Test]
        public async Task CanNavigateToNewAssetForm()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            // login
            var login = new LoginPage(page);
            await login.GoToAsync();
            await login.LoginAsync("admin", "password");

            // assets → new asset form
            var assets = new AssetsPage(page);
            await assets.GoToCreateAssetFormAsync();

            // Assert the Asset Tag input is visible
            Assert.That(await page.Locator("#asset_tag").IsVisibleAsync(), Is.True);
        }

        [Test]
        public async Task CanCreateMacBookAsset()
        {
            using var pw = await Playwright.CreateAsync();
            await using var browser = await pw.Chromium.LaunchAsync(new() { Headless = false });
            var ctx = await browser.NewContextAsync();
            var page = await ctx.NewPageAsync();

            var login = new LoginPage(page);
            await login.GoToAsync();
            await login.LoginAsync("admin", "password");

            var assets = new AssetsPage(page);
            await assets.GoToCreateAssetFormAsync();

            var (assetTag, assetName) = await assets.FillBasicFieldsAsync();
            await assets.SetModelAsync();
            await assets.SetStatusAsync();
            var checkedOutTo = await assets.CheckoutToRandomUserAsync();
            await assets.SaveAsync();
            
        }
    }
}
  
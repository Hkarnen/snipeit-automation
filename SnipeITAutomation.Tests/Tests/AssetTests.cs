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

            var assetTag = await assets.GetPrefilledAssetTagAsync();
            Assert.That(await page.Locator("#asset_tag").InputValueAsync(), Is.EqualTo(assetTag));
            // Set Model = MacBook Pro 13"
            await assets.SetModelAsync();
            var modelText = (await page.Locator("#select2-model_select_id-container").InnerTextAsync()).Trim();
            Assert.That(modelText, Does.Contain("Macbook Pro 13").IgnoreCase);
            // Set Status = Ready to Deploy
            await assets.SetStatusAsync();
            var statusText = (await page.Locator("#select2-status_select_id-container").InnerTextAsync()).Trim();
            Assert.That(statusText, Does.Contain("Ready to Deploy").IgnoreCase);
            // Checkout to random user
            var checkedOutTo = await assets.CheckoutToRandomUserAsync();
            var checkoutText = (await page.Locator("#select2-assigned_user_select-container").InnerTextAsync()).Trim();
            Assert.That(checkoutText, Does.Contain(checkedOutTo).IgnoreCase);

            await assets.SaveAsync();

        }

        [Test]
        public async Task AllSteps()
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

            var assetTag = await assets.GetPrefilledAssetTagAsync();
            Assert.That(await page.Locator("#asset_tag").InputValueAsync(), Is.EqualTo(assetTag));
            // Set Model = MacBook Pro 13"
            await assets.SetModelAsync();
            var modelText = (await page.Locator("#select2-model_select_id-container").InnerTextAsync()).Trim();
            Assert.That(modelText, Does.Contain("Macbook Pro 13").IgnoreCase);
            // Set Status = Ready to Deploy
            await assets.SetStatusAsync();
            var statusText = (await page.Locator("#select2-status_select_id-container").InnerTextAsync()).Trim();
            Assert.That(statusText, Does.Contain("Ready to Deploy").IgnoreCase);
            // Checkout to random user
            var checkedOutTo = await assets.CheckoutToRandomUserAsync();
            var checkoutText = (await page.Locator("#select2-assigned_user_select-container").InnerTextAsync()).Trim();
            Assert.That(checkoutText, Does.Contain(checkedOutTo).IgnoreCase);

            await assets.SaveAsync();
            // Go to asset list
            await assets.GoToAsync();
            // Search for the asset by Asset Tag
            await assets.SearchForAssetAsync(assetTag);
            // Assert the asset appears in the search results
            var row = page.Locator("table tbody tr").Filter(new() { HasTextString = assetTag }).First;
            await row.WaitForAsync();
            Assert.That(await row.IsVisibleAsync(), Is.True);


            // Open asset details
            await row.Locator("a[href*='/hardware/']").First.ClickAsync();
            await page.WaitForURLAsync("**/hardware/*");

            await page.Locator("#details").WaitForAsync();   // ensure details tab content is visible
            var details = page.Locator("#details");

            var detailsText = await details.InnerTextAsync();
            // asset tag
            Assert.That(detailsText, Does.Contain(assetTag));

            // status
            Assert.That(detailsText, Does.Contain("Ready to Deploy"));

            // model
            Assert.That(detailsText, Does.Contain("Macbook Pro 13"));

            // checked-out user
            var parts = checkedOutTo.Split('#');
            var userNameOnly = parts[0].Split('(')[0].Trim();
            var userIdOnly = parts.Length > 1 ? parts[1].TrimEnd(')') : string.Empty;

            Assert.That(detailsText, Does.Contain(userNameOnly).IgnoreCase);
            Assert.That(detailsText, Does.Contain(userIdOnly));

            // validate details in history tab
            await page.ClickAsync("a[href='#history']");
            // wait for the history table to render
            var historyTable = page.Locator("#assetHistory");
            await historyTable.WaitForAsync();

           // pick the "checkout" row
            var checkoutRow = historyTable.Locator("tbody tr").Filter(new() { HasTextString = "checkout" }).First;
            await checkoutRow.WaitForAsync();

            // target user
            var userLinks = checkoutRow.Locator("a[href*='/users/']");
            await userLinks.First.WaitForAsync();
            var targetText = (await userLinks.Last.InnerTextAsync()).Trim();
            Assert.That(targetText, Does.Contain(userNameOnly).IgnoreCase);

        }
    }
}
  
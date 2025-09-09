using System.Threading.Tasks;
using Microsoft.Playwright;

namespace SnipeITAutomation.Tests.Pages
{
    public class AssetsPage
    {
        private readonly IPage _page;
        public AssetsPage(IPage page) => _page = page;

        /// <summary>
        /// Opens the Assets list.
        /// </summary>
        public async Task NavigateToAssetsAsync()
        {
            await _page.GotoAsync("https://demo.snipeitapp.com/hardware");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Wait for something stable on the list page (either table or "New Asset" button)
            var tableOrButton = _page.Locator("table, a:has-text('New Asset'), button:has-text('New Asset')");
            await tableOrButton.First.WaitForAsync();
        }

        /// <summary>
        /// Open the New Asset form.
        /// </summary>
        public async Task GoToCreateAssetFormAsync()
        {
            // Prefer clicking the UI element if present
            var newAssetLink = _page.Locator("a:has-text('New Asset'), button:has-text('New Asset')");
            if (await newAssetLink.First.IsVisibleAsync())
            {
                await newAssetLink.First.ClickAsync();
            }
            else
            {
                // Fallback to direct route
                await _page.GotoAsync("https://demo.snipeitapp.com/hardware/create");
            }

            await _page.WaitForURLAsync("**/hardware/create");
            // Sanity: the form should show fields like "Asset Tag"
            await _page.GetByLabel("Asset Tag").WaitForAsync();
        }

        // Placeholder for additional methods to interact with the Assets page
        // public Task FillBasicFieldsAsync(string assetTag, string assetName) { ... }
        // public Task SetModelAsync(string modelName) { ... }                         // e.g., "MacBook Pro 13\""
        // public Task SetStatusAsync(string statusName) { ... }                       // "Ready to Deploy"
        // public Task<string?> CheckoutToRandomUserAsync() { ... }
        // public Task SaveAsync() { ... }
        // public Task FindAssetInListAsync(string query) { ... }
        // public Task OpenAssetFromListAsync(string query) { ... }
        // public Task ValidateDetailsOnShowPageAsync(...) { ... }
        // public Task ValidateHistoryTabAsync(...) { ... }
    }
}

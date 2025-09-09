using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace SnipeITAutomation.Tests.Pages
{
    public class AssetsPage
    {
        private readonly IPage _page;
        public AssetsPage(IPage page) => _page = page;

        /// <summary>
        /// Go straight to the new asset form
        /// </summary>
        public async Task GoToCreateAssetFormAsync()
        {
            // Simply use direct link
            await _page.GotoAsync("https://demo.snipeitapp.com/hardware/create");
            await _page.Locator("#asset_tag").WaitForAsync();
        }

        /// <summary>
        /// Fills Asset Tag and Asset Name with unique values
        /// </summary>
        public async Task<(string AssetTag, string AssetName)> FillBasicFieldsAsync(
            string assetTagPrefix = "MBP13",
            string assetNamePrefix = "MacBook Pro 13")
        {
            var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var assetTag = $"{assetTagPrefix}-{suffix}";
            var assetName = $"{assetNamePrefix} {suffix}";

            // Asset Tag
            var assetTagInput = _page.Locator("#asset_tag");
            await assetTagInput.FillAsync(assetTag);

            // Asset Name
            var nameInput = _page.GetByLabel("Asset Name");
            await nameInput.FillAsync(assetName);

            return (assetTag, assetName);
        }

        /// <summary>
        /// Selects Model = MacBook Pro 13"
        /// </summary>
        public async Task SetModelAsync()
        {
            await _page.ClickAsync("#select2-model_id-container");
            await _page.FillAsync(".select2-search__field", "MacBook Pro 13");
            await _page.ClickAsync(".select2-results__option:has-text(\"MacBook Pro 13\")");
        }

        /// <summary>
        /// Sets Status = Ready to Deploy
        /// </summary>
        public async Task SetStatusAsync()
        {
            await _page.ClickAsync("#select2-status_id-container");
            await _page.FillAsync(".select2-search__field", "Ready to Deploy");
            await _page.ClickAsync(".select2-results__option:has-text(\"Ready to Deploy\")");
        }

        /// <summary>
        /// Checks out asset to a random user and returns the user's name
        /// </summary>
        public async Task<string> CheckoutToRandomUserAsync()
        {
            await _page.ClickAsync("#select2-assigned_to-container");

            var options = _page.Locator(".select2-results__option");
            await options.First.WaitForAsync();

            // Pick first real option after the placeholder
            var text = (await options.Nth(1).InnerTextAsync()).Trim();
            await options.Nth(1).ClickAsync();
            return text;
        }

        /// <summary>
        /// Clicks Save and waits for navigation.
        /// </summary>
        public async Task SaveAsync()
        {
            await _page.ClickAsync("button:has-text(\"Save\")");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
    }
}

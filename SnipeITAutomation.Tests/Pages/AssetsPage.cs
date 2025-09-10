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
        /// Navigate to the Assets page
        /// </summary>
        public async Task GoToAsync() =>
            await _page.GotoAsync("https://demo.snipeitapp.com/hardware");

        /// <summary>
        /// Go straight to the new asset form
        /// </summary>
        public async Task GoToCreateAssetFormAsync()
        {
            // Simply use direct link
            await _page.GotoAsync("https://demo.snipeitapp.com/hardware/create");
            await _page.Locator("#asset_tag").WaitForAsync();
        }

        public async Task SearchForAssetAsync(string assetTag)
        {
            var searchBox = _page.Locator("input.search-input");
            await searchBox.FillAsync(assetTag);
            await searchBox.PressAsync("Enter");
        }

        /// <summary>
        /// Fills Asset Tag and Asset Name with unique values
        /// </summary>
        public async Task<string> GetPrefilledAssetTagAsync()
        {
            // The create form prepopulates #asset_tag
            var tag = (await _page.Locator("#asset_tag").InputValueAsync()).Trim();
            return tag;
        }

        /// <summary>
        /// Selects Model = MacBook Pro 13"
        /// </summary>
        public async Task SetModelAsync()
        {
            var target = "MacBook Pro 13\"";
            // Open the Select2 dropdown
            await _page.ClickAsync("#select2-model_select_id-container");
            // Type into the search box
            await _page.FillAsync(".select2-search__field", target);
            // Click the matching option
            await _page.ClickAsync(".select2-results__option:has-text(\"MacBook Pro 13\")");
        }

        /// <summary>
        /// Sets Status = Ready to Deploy
        /// </summary>
        public async Task SetStatusAsync()
        {
            var target = "Ready to Deploy";
            // Open the Select2 dropdown
            await _page.ClickAsync("#select2-status_select_id-container");
            // Type into the search box
            await _page.FillAsync(".select2-search__field", target);
            // Click the matching option
            await _page.ClickAsync(".select2-results__option:has-text(\"Ready to Deploy\")");
            // Wait for the checkout section to appear after status is set
            await _page.Locator("#assignto_selector").WaitForAsync();
        }

        /// <summary>
        /// Checks out asset to a random user and returns the user's name
        /// </summary>
        public async Task<string> CheckoutToRandomUserAsync()
        {
            await _page.ClickAsync("#assignto_selector .btn:has-text(\"User\")");
            await _page.ClickAsync("#select2-assigned_user_select-container");

            var options = _page.Locator(".select2-results__option[role='option']:not([aria-disabled='true'])");
            await options.First.WaitForAsync();

            var count = await options.CountAsync();
            var idx = new Random().Next(0, count);        // 0..count-1
            var chosen = options.Nth(idx);
            var text = (await chosen.InnerTextAsync()).Trim();
            await chosen.ClickAsync();
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace OrleansShopping.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ProductsModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ProductsModel> _logger;

        public HashSet<ProductDetails>? Products;
        public InventoryService InventoryService { get; set; } = null!;

        public ProductsModel(ILogger<ProductsModel> logger, InventoryService inventoryService)
        {
            _logger = logger;
            InventoryService = inventoryService;
        }

        public async Task<IActionResult> OnGet()
        {
            Products = (await InventoryService.GetAllProductsAsync()).OrderBy(x => x.Name).ToHashSet();

            return Page();
        }
    }
}
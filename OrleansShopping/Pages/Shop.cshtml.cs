using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OrleansShopping.Pages
{
    public class ShopModel : PageModel
    {
        private readonly InventoryService InventoryService;
        public ShoppingCartService ShoppingCartService { get; set; } = null!;
        private readonly ILogger<ShopModel> _logger;
        public HashSet<ProductDetails> Products;
        public HashSet<CartItem> CartItems;
        public int CartCount;

        public ShopModel(ILogger<ShopModel> logger,
            InventoryService inventoryService,
            ShoppingCartService cartService)
        {
            this.InventoryService = inventoryService;
            this.ShoppingCartService = cartService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            Products = await this.InventoryService.GetAllProductsAsync();
            CartCount = await ShoppingCartService.GetCartCountAsync();

            return Page();
        }


        public async Task<IActionResult> OnPost([FromForm(Name ="item.Id")]string ProductId)
        {
            // TODO: Add Get Item By ID method
            Products = await this.InventoryService.GetAllProductsAsync();
            CartCount = await ShoppingCartService.GetCartCountAsync();

            var product = Products?.FirstOrDefault(p => p.Id == ProductId);
            if (product is null)
            {
                return Page();
            }

            if (await ShoppingCartService.AddOrUpdateItemAsync(1, product))
            {
                CartCount = await ShoppingCartService.GetCartCountAsync();
            }

            Products = await this.InventoryService.GetAllProductsAsync();

            // This prevents all the hidden form field ids from being the same on reload - otherwise they'll get repopulated from model state
            // Could also just redirect to the OnGet
            ModelState.Clear();
            return Page();
        }
    }
}
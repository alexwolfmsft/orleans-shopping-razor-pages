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
            CartItems = await this.ShoppingCartService.GetAllItemsAsync();
            CartCount = await ShoppingCartService.GetCartCountAsync();
            Products.ToList().ForEach(x => x.InCart = CartItems.Any(y => y.Product.Id == x.Id));

            return Page();
        }


        public async Task<IActionResult> OnPost([FromForm(Name ="item.Id")]string ProductId)
        {
            // TODO: Add Get Item By ID method to avoid this call
            Products = await this.InventoryService.GetAllProductsAsync();

            var product = Products?.FirstOrDefault(p => p.Id == ProductId);
            if (product is null)
            {
                return Page();
            }

            await ShoppingCartService.AddOrUpdateItemAsync(1, product);

            // This redirect prevents us from having to repopulate everything on the Post
            // It also adheres to post -> redirect -> get pattern
            return RedirectToPage();
        }
    }
} 
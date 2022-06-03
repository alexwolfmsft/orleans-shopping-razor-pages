using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OrleansShopping.Pages
{
    public class CartModel : PageModel
    {
        public HashSet<CartItem> CartItems { get; set; } = new HashSet<CartItem>();

        private ShoppingCartService cartService;
        private InventoryService inventoryService;

        public CartModel(ShoppingCartService cartService, InventoryService productService)
        {
            this.cartService = cartService;
            this.inventoryService = productService;
        }

        public async Task<IActionResult> OnGet()
        {
            CartItems = await cartService.GetAllItemsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await cartService.EmptyCartAsync();
            return Page();
        }
    }
}

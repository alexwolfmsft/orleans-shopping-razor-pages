using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OrleansShopping.Pages
{
    public class EditProductModel : PageModel
    {
        [BindProperty]
        public ProductDetails Product { get; set; } = new ProductDetails();

        public ProductService ProductService { get; set; } = null!;

        public EditProductModel(ProductService productService)
        {
            ProductService = productService;
        }

        public void OnGet()
        {
            var bogusProduct = new ProductDetails();
            var faker = bogusProduct.GetBogusFaker();
            Product = faker.Generate();
        }

        public async Task<IActionResult> OnPost()
        {
            await ProductService.CreateOrUpdateProductAsync(Product);

            return RedirectToPage("Products");
        }
    }
}

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Orleans.ShoppingCart.Grains;

[Reentrant]
public sealed class ShoppingCartGrain : Grain, IShoppingCartGrain
{
    private readonly IPersistentState<Dictionary<string, CartItem>> _cart;
    private readonly IPersistentState<ProductDetails> _product;

    public ShoppingCartGrain(
        [PersistentState(
            stateName: "ShoppingCart",
            storageName: "shopping-cart")]
        IPersistentState<Dictionary<string, CartItem>> cart, [PersistentState(
            stateName: "Product",
            storageName: "shopping-cart")]
        IPersistentState<ProductDetails> product)
    { 
        _cart = cart; 
        _product = product; }

    async Task<bool> IShoppingCartGrain.AddOrUpdateItemAsync(int quantity, ProductDetails product)
    {
        var products = GrainFactory.GetGrain<IProductGrain>(product.Id);
   
        int? adjustedQuantity = null;
        if (_cart.State.TryGetValue(product.Id, out var existingItem))
        {
            adjustedQuantity = quantity - existingItem.Quantity;
        }

        var (isAvailable, claimedProduct) =
            await products.TryTakeProductAsync(adjustedQuantity ?? quantity);
        if (isAvailable && claimedProduct is not null)
        {
            var item = ToCartItem(quantity, claimedProduct);
            _cart.State[claimedProduct.Id] = item;

            await _cart.WriteStateAsync();
            return true;
        }

        return false;
    }

    Task IShoppingCartGrain.EmptyCartAsync()
    {
        //_cart.State.Values.ToList().ForEach(x => {
        //    x.Product.InCart = false;
        //    _product.State = x.Product;
        //    _product.WriteStateAsync();
        //});
        _cart.State.Clear();
        return _cart.ClearStateAsync();
    }

    Task<HashSet<CartItem>> IShoppingCartGrain.GetAllItemsAsync() =>
        Task.FromResult(_cart.State.Values.ToHashSet());

    Task<int> IShoppingCartGrain.GetTotalItemsInCartAsync() =>
        Task.FromResult(_cart.State.Count);

    async Task IShoppingCartGrain.RemoveItemAsync(ProductDetails product)
    {
        var products = GrainFactory.GetGrain<IProductGrain>(product.Id);
        await products.ReturnProductAsync(product.Quantity);

        if (_cart.State.Remove(product.Id))
        {
            await _cart.WriteStateAsync();
        }
    }

    private CartItem ToCartItem(int quantity, ProductDetails product) =>
        new(this.GetPrimaryKeyString(), quantity, product);
}

namespace AppUI.Models.ViewModels
{
    public class IndexShopCartsVm
    {
        public required string Customer { get; set; }
        public required int CustomerId { get; set; }
        public List<ShopCartVm>? ListShopCarts { get; set; }
    }
}

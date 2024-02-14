namespace AppUI.Models.ViewModels
{
    public class IndexOrdersVm
    {
        public required string Customer { get; set; }
        public required int CustomerId { get; set; }
        public List<OrderVm>? ListOrders { get; set; }
    }
}

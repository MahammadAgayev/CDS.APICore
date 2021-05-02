namespace CDS.APICore.Models.Order
{
    public class AddOrderRequestItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }
}

namespace CDS.APICore.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public ItemCategory ItemCategory { get; set; }
    }
}

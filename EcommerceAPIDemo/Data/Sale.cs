namespace EcommerceAPIDemo.Data;

public class Sale
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public List<GameProduct> GamesPurchased { get; set; }
    public double SubTotal { get; set; }
    public double SalesTax { get; set; }
    public double Total { get; set; }
}

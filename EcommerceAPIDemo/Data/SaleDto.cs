namespace EcommerceAPIDemo.Data;

public class SaleDto
{
    public List<GameProduct> GamesPurchased { get; set; }
    public CreditCardTypes creditCardType { get; set; }
    public int LastFourDigitsOfPaymentCard { get; set; }
    public double SubTotal { get; set; }
    public double SalesTax { get; set; }
    public double Total { get; set; }
}

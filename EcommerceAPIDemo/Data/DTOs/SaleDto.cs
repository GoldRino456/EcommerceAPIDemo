using EcommerceAPIDemo.Data.Models;

namespace EcommerceAPIDemo.Data.DTOs;

public class SaleDto
{
    public List<GameProduct> GamesPurchased { get; } = [];
    public CreditCardTypes creditCardType { get; set; }
    public int LastFourDigitsOfPaymentCard { get; set; }
    public double SubTotal { get; set; }
    public double SalesTax { get; set; }
    public double Total { get; set; }
}

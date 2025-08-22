using EcommerceAPIDemo.Data;

namespace EcommerceAPIDemo.Services;

public interface ISalesService
{
    //READ Operations
    public List<Sale>? GetAllSales();
    public List<Sale>? GetTodaysSales();
    public List<Sale>? GetSalesForSpecificProduct(int gameProductId);
    public List<Sale>? GetAllSalesInDateRange(DateTime? startDateInclusive, DateTime? endDateInclusive);
    public Sale? GetSaleById(int saleId);
    public List<Sale>? GetSalesByLastFourOfPaymentCard(int cardLastFourDigits);

    //CREATE Operations
    public Sale CreateSale(SaleDto dto);

    //UPDATE Operations
    public Sale? RefundExistingSale(int saleId);
    public Sale? RefundPartialExistingSale(int saleId, double refundAmount);
}

public class SalesService : ISalesService
{
    private readonly SalesDbContext _salesDbContext;

    public SalesService(SalesDbContext salesDbContext)
    {
        _salesDbContext = salesDbContext;
    }

    public Sale CreateSale(SaleDto dto)
    {
        Sale newSale = ConvertDtoToSale(dto);

        UpdateSaleTransactionTimestamps(newSale);
        newSale.IsRefund = false;
        newSale.IsPartialRefund = false;
        newSale.ActualTransactionValue = newSale.Total;

        var savedSale = _salesDbContext.Sales.Add(newSale);
        _salesDbContext.SaveChanges();
        
        return savedSale.Entity;
    }

    public List<Sale>? GetAllSales()
    {
        var sales = _salesDbContext.Sales.ToList();

        if(sales.Count <= 0)
        {
            return null;
        }

        return sales;
    }

    public List<Sale>? GetAllSalesInDateRange(DateTime? startDateInclusive, DateTime? endDateExclusive)
    {
        bool isStartValueNull = startDateInclusive == null;
        bool isEndValueNull = endDateExclusive == null;
        List<Sale> sales;

        if(isStartValueNull && isEndValueNull)
        {
            return null;
        }

        if(isStartValueNull) //End Value Only
        {
            sales = _salesDbContext.Sales.Where(sale => sale.TransactionDate < endDateExclusive).ToList();
        }
        else if (isEndValueNull) //Start Value Only
        {
            sales = _salesDbContext.Sales.Where(sale => sale.TransactionDate >= startDateInclusive).ToList();
        }
        else //Both Bounds
        {
            sales = _salesDbContext.Sales.Where(sale => (sale.TransactionDate < endDateExclusive) && (sale.TransactionDate >= startDateInclusive)).ToList();
        }

        if(sales.Count <= 0)
        {
            return null;
        }

        return sales;
    }

    public Sale? GetSaleById(int saleId)
    {
        return _salesDbContext.Sales.Find(saleId);
    }

    public List<Sale>? GetSalesByLastFourOfPaymentCard(int cardLastFourDigits)
    {
        List<Sale> sales = _salesDbContext.Sales.Where(sale => sale.LastFourDigitsOfPaymentCard == cardLastFourDigits).ToList();

        if(sales.Count <= 0)
        {
            return null;
        }

        return sales;
    }

    public List<Sale>? GetSalesForSpecificProduct(int gameProductId)
    {
        GameProduct? game = _salesDbContext.GameProducts.Find(gameProductId);

        if(game == null)
        {
            return null;
        }

        List<Sale> sales = _salesDbContext.Sales.Where(sale => sale.GamesPurchased.Contains(game)).ToList();

        if (sales.Count <= 0)
        {
            return null;
        }

        return sales;
    }

    public List<Sale>? GetTodaysSales()
    {
        List<Sale> sales = _salesDbContext.Sales.Where(sale => sale.TransactionDate.Date == DateTime.Now.Date).ToList();

        if (sales.Count <= 0)
        {
            return null;
        }

        return sales;
    }

    public Sale? RefundExistingSale(int saleId)
    {
        Sale? sale = _salesDbContext.Sales.Find(saleId);

        if(sale == null)
        {
            return null;
        }

        sale.ActualTransactionValue = 0.0;
        sale.IsRefund = true;
        sale.IsPartialRefund = false;
        UpdateSaleTransactionTimestamps(sale);

        var updatedSale = _salesDbContext.Sales.Update(sale);
        _salesDbContext.SaveChanges();

        return updatedSale.Entity;
    }

    public Sale? RefundPartialExistingSale(int saleId, double refundAmount)
    {
        Sale? sale = _salesDbContext.Sales.Find(saleId);

        if (sale == null || refundAmount <= 0.0 || refundAmount >= sale.ActualTransactionValue)
        {
            return null;
        }

        sale.ActualTransactionValue -= refundAmount;
        sale.IsPartialRefund = true;
        UpdateSaleTransactionTimestamps(sale);

        var updatedSale = _salesDbContext.Sales.Update(sale);
        _salesDbContext.SaveChanges();

        return updatedSale.Entity;
    }

    private void UpdateSaleTransactionTimestamps(Sale sale)
    {
        if(sale.TransactionDate == default)
        {
            sale.TransactionDate = DateTime.Now;
            sale.TransactionLastUpdatedDate = DateTime.Now;
        }
        else
        {
            sale.TransactionLastUpdatedDate = DateTime.Now;
        }
    }

    private Sale ConvertDtoToSale(SaleDto dto)
    {
        Sale newSale = new()
        {
            creditCardType = dto.creditCardType,
            LastFourDigitsOfPaymentCard = dto.LastFourDigitsOfPaymentCard,
            SubTotal = dto.SubTotal,
            SalesTax = dto.SalesTax,
            Total = dto.Total
        };

        if (dto.PurchasedGameIds != null)
        {
            foreach (var id in dto.PurchasedGameIds)
            {
                var selectedGame = _salesDbContext.GameProducts.Find(id);
                if (selectedGame != null)
                {
                    newSale.GamesPurchased.Add(selectedGame);
                }
            }
        }

        return newSale;
    }
}

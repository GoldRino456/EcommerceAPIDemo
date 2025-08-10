using EcommerceAPIDemo.Data;

namespace EcommerceAPIDemo.Services;

public interface ISalesService
{
    //READ Operations
    public List<Sale> GetAllSales();
    public List<Sale> GetTodaysSales();
    public List<Sale> GetSalesForSpecificProduct(int gameProductId);
    public List<Sale> GetSalesForSpecificCategory(int gameCategoryId);
    public List<Sale> GetAllSalesInDateRange(DateTime? startDateInclusive, DateTime? endDateInclusive);
    public Sale GetSaleById(int saleId);
    public List<Sale> GetSalesByLastFourOfPaymentCard(int cardLastFourDigits);

    //CREATE Operations
    public Sale CreateSale(SaleDto newSale);

    //UPDATE Operations
    public Sale RefundExistingSale(int saleId);
    public Sale RefundPartialExistingSale(int saleId, double refundAmount);
}

public class SalesService
{

}

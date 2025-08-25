using EcommerceAPIDemo.Data;
using EcommerceAPIDemo.Data.DTOs;
using EcommerceAPIDemo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPIDemo.Services;

public interface ISalesService
{
    //READ Operations
    public IQueryable<Sale>? GetAllSales();
    public Task<Sale?> GetSale(int saleId);

    //CREATE Operations
    public Task<Sale> CreateSale(SaleDto dto);

    //UPDATE Operations
    public Task<Sale> RefundExistingSale(int saleId);
    public Task<Sale> RefundPartialExistingSale(int saleId, double refundAmount);
}

public class SalesService : ISalesService
{
    private readonly SalesDbContext _salesDbContext;

    public SalesService(SalesDbContext salesDbContext)
    {
        _salesDbContext = salesDbContext;
    }

    public async Task<Sale> CreateSale(SaleDto dto)
    {
        Sale newSale = await ConvertDtoToSale(dto);

        UpdateSaleTransactionTimestamps(newSale);
        newSale.IsRefund = false;
        newSale.IsPartialRefund = false;
        newSale.ActualTransactionValue = newSale.Total;

        var savedSale = _salesDbContext.Sales.Add(newSale);
        await _salesDbContext.SaveChangesAsync();
        
        return savedSale.Entity;
    }

    public IQueryable<Sale>? GetAllSales()
    {
        var sales = _salesDbContext.Sales
            .Include(s => s.GamesPurchased)
            .OrderBy(s => s.Id);

        if(!sales.Any())
        {
            return null;
        }

        return sales;
    }

    public async Task<Sale?> GetSale(int saleId)
    {
        return await _salesDbContext.Sales
            .Include(s => s.GamesPurchased)
            .FirstOrDefaultAsync(s => s.Id == saleId);
    }

    public async Task<Sale> RefundExistingSale(int saleId)
    {
        Sale sale = await _salesDbContext.Sales
            .Include(s => s.GamesPurchased)
            .FirstAsync(s => s.Id == saleId);

        //This is where we'd trigger some response from a payment processor
        sale.ActualTransactionValue = 0.0;
        sale.IsRefund = true;
        sale.IsPartialRefund = false;
        UpdateSaleTransactionTimestamps(sale);

        var updatedSale = _salesDbContext.Sales.Update(sale);
        await _salesDbContext.SaveChangesAsync();

        return updatedSale.Entity;
    }

    public async Task<Sale> RefundPartialExistingSale(int saleId, double refundAmount)
    {
        Sale sale = await _salesDbContext.Sales
            .Include(s => s.GamesPurchased)
            .FirstAsync(s => s.Id == saleId);

        //This is where we'd trigger some response from a payment processor
        sale.ActualTransactionValue -= refundAmount;
        sale.IsPartialRefund = true;
        UpdateSaleTransactionTimestamps(sale);

        var updatedSale = _salesDbContext.Sales.Update(sale);
        await _salesDbContext.SaveChangesAsync();

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

    private async Task<Sale> ConvertDtoToSale(SaleDto dto)
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
                var selectedGame = await _salesDbContext.GameProducts
                    .Include(p => p.Id == id)
                    .FirstOrDefaultAsync();

                if (selectedGame != null)
                {
                    newSale.GamesPurchased.Add(selectedGame);
                }
            }
        }

        return newSale;
    }
}

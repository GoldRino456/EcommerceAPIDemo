namespace EcommerceAPIDemo.Controllers;

public class GameProductQuery
{
    public PaginationParams Pagination { get; set; } = new();
    public GameProductFilterParams GameProductFilters { get; set; } = new();
}

public class GameProductFilterParams
{
    public int? CategoryId { get; set; }
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
}


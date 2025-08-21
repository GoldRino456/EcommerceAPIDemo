namespace EcommerceAPIDemo.Data.Models;

public class GameProductCategory
{
    public int GameProductId { get; set; }
    public GameProduct GameProduct { get; set; }
    public int GameCategoryId { get; set; }
    public GameCategory GameCategory { get; set; }
}

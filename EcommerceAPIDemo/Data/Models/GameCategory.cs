namespace EcommerceAPIDemo.Data.Models
{
    public class GameCategory
    {
        public int GameCategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<GameProductCategory> ProductCategories { get; set; }
    }
}

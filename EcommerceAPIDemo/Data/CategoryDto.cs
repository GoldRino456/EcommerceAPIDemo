namespace EcommerceAPIDemo.Data;

public class CategoryDto
{
    public string Name { get; set; }
    public List<GameProduct>? GamesInCategory { get; set; }
}

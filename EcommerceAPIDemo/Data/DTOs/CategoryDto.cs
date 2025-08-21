using EcommerceAPIDemo.Data.Models;

namespace EcommerceAPIDemo.Data.DTOs;

public class CategoryDto
{
    public string Name { get; set; }
    public List<GameProduct> GamesInCategory { get; } = [];
}

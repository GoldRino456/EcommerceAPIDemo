using EcommerceAPIDemo.Data;

namespace EcommerceAPIDemo.Services;

public interface IGamesService
{
    //READ Operations
    public List<GameProduct> GetAllGames(); //Pagination
    public List<GameCategory> GetAllCategories(); //Pagination
    public List<GameProduct> GetAllGamesInCategory(int categoryId); //Pagination
    public List<GameCategory> GetAllGamesWithinPriceRange(double? minInclusive, double? maxExclusive); //Pagination
    public GameProduct GetGame(int gameProductId);

    //CREATE Operations
    public GameProduct CreateGame(GameDto dto);
    public GameCategory CreateCategory(CategoryDto dto);

    //UPDATE Operations
    public GameProduct UpdateGame(int gameProductId, GameDto dto);
    public GameCategory UpdateCategory(int gameCategoryId, GameCategory dto);
}

public class GamesService
{
}

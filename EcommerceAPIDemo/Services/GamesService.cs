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
    public GameProduct? UpdateGame(int gameProductId, GameDto dto);
    public GameCategory? UpdateCategory(int gameCategoryId, CategoryDto dto);
}

public class GamesService : IGamesService
{
    private readonly SalesDbContext _salesDbContext;

    public GamesService(SalesDbContext salesDbContext)
    {
        _salesDbContext = salesDbContext;
    }

    public GameCategory CreateCategory(CategoryDto dto)
    {
        GameCategory newCategory = ConvertDtoToGameCategory(dto);

        var savedCategory = _salesDbContext.GameCategories.Add(newCategory);
        _salesDbContext.SaveChanges();

        return savedCategory.Entity;
    }

    public GameProduct CreateGame(GameDto dto)
    {
        GameProduct newGame = ConvertDtoToGameProduct(dto);

        var savedGame = _salesDbContext.GameProducts.Add(newGame);
        _salesDbContext.SaveChanges();

        return savedGame.Entity;
    }

    public List<GameCategory> GetAllCategories()
    {
        throw new NotImplementedException();
    }

    public List<GameProduct> GetAllGames()
    {
        throw new NotImplementedException();
    }

    public List<GameProduct> GetAllGamesInCategory(int categoryId)
    {
        throw new NotImplementedException();
    }

    public List<GameCategory> GetAllGamesWithinPriceRange(double? minInclusive, double? maxExclusive)
    {
        throw new NotImplementedException();
    }

    public GameProduct GetGame(int gameProductId)
    {
        throw new NotImplementedException();
    }

    public GameCategory? UpdateCategory(int gameCategoryId, CategoryDto dto)
    {
        var existingCategory = _salesDbContext.GameCategories.Find(gameCategoryId);

        if(existingCategory == null)
        {
            return null;
        }

        GameCategory newCategory = ConvertDtoToGameCategory(dto);
        newCategory.Id = existingCategory.Id;

        _salesDbContext.GameCategories.Entry(existingCategory).CurrentValues.SetValues(newCategory);
        _salesDbContext.SaveChanges();

        return existingCategory;
    }

    public GameProduct? UpdateGame(int gameProductId, GameDto dto)
    {
        throw new NotImplementedException();
    }

    private static GameCategory ConvertDtoToGameCategory(CategoryDto dto)
    {
        return new()
        {
            Name = dto.Name,
            GamesInCategory = new()
        };
    }

    private static GameProduct ConvertDtoToGameProduct(GameDto dto)
    {
        return new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Categories = dto.Categories,
            Sales = new(),
            Developer = dto.Developer,
            Publisher = dto.Publisher,
            ReleaseDate = dto.ReleaseDate,
            Price = dto.Price,
            FileSize = dto.FileSize,
            SystemRequirements = dto.SystemRequirements
        };
    }
}

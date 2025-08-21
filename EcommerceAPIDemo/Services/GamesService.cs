using EcommerceAPIDemo.Data;
using EcommerceAPIDemo.Data.DTOs;
using EcommerceAPIDemo.Data.Models;

namespace EcommerceAPIDemo.Services;

public interface IGamesService
{
    //READ Operations
    public List<GameProduct>? GetAllGames(); //Pagination
    public List<GameCategory>? GetAllCategories(); //Pagination
    public List<GameProduct>? GetAllGamesInCategory(int categoryId); //Pagination
    public List<GameProduct>? GetAllGamesWithinPriceRange(double? minInclusive, double? maxExclusive); //Pagination
    public GameProduct? GetGame(int gameProductId);
    public GameCategory? GetCategory(int categoryId);

    //CREATE Operations
    public GameProduct CreateGame(GameDto dto);
    public GameCategory CreateCategory(CategoryDto dto);

    //UPDATE Operations
    public GameProduct UpdateGame(int gameProductId, GameDto dto);
    public GameCategory UpdateCategory(int gameCategoryId, CategoryDto dto);
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

    public List<GameCategory>? GetAllCategories()
    {
        var categories = _salesDbContext.GameCategories.ToList();

        if(categories.Count <= 0)
        {
            return null;
        }

        return categories;
    }

    public List<GameProduct>? GetAllGames()
    {
        var games = _salesDbContext.GameProducts.ToList();

        if (games.Count <= 0)
        {
            return null;
        }

        return games;
    }

    public List<GameProduct>? GetAllGamesInCategory(int categoryId)
    {
        var category = GetCategory(categoryId);

        var games = _salesDbContext.GameProducts.Where(product => product.Categories.Contains(category)).ToList();

        if(games.Count <= 0)
        {
            return null;
        }

        return games;
    }

    public List<GameProduct>? GetAllGamesWithinPriceRange(double? minInclusive, double? maxExclusive)
    {

        bool isMinValueNull = minInclusive == null;
        bool isMaxValueNull = maxExclusive == null;
        List<GameProduct> games;
        
        if(isMinValueNull) //Max Value Only
        {
            games = _salesDbContext.GameProducts.Where(game => game.Price < maxExclusive).ToList();
        }
        else if(isMaxValueNull) //Min Value Only
        {
            games = _salesDbContext.GameProducts.Where(game => game.Price >= minInclusive).ToList();
        }
        else //Both Bounds
        {
            games = _salesDbContext.GameProducts.Where(game => (game.Price < maxExclusive) && (game.Price >= minInclusive)).ToList();
        }

        if(games.Count <= 0)
        {
            return null;
        }
        
        return games;
    }

    public GameProduct? GetGame(int gameProductId)
    {
        return _salesDbContext.GameProducts.Find(gameProductId);
    }

    public GameCategory? GetCategory(int categoryId)
    {
        return _salesDbContext.GameCategories.Find(categoryId);
    }

    public GameCategory UpdateCategory(int gameCategoryId, CategoryDto dto)
    {
        var existingCategory = _salesDbContext.GameCategories.Find(gameCategoryId);


        GameCategory newCategory = ConvertDtoToGameCategory(dto);
        newCategory.GameCategoryId = existingCategory.GameCategoryId;

        _salesDbContext.GameCategories.Entry(existingCategory).CurrentValues.SetValues(newCategory);
        _salesDbContext.SaveChanges();

        return existingCategory;
    }

    public GameProduct UpdateGame(int gameProductId, GameDto dto)
    {
        var existingGame = _salesDbContext.GameProducts.Find(gameProductId);

        GameProduct newGame = ConvertDtoToGameProduct(dto);
        newGame.GameProductId = existingGame.GameProductId;

        _salesDbContext.GameProducts.Entry(existingGame).CurrentValues.SetValues(newGame);
        _salesDbContext.SaveChanges();

        return existingGame;
    }

    private GameCategory ConvertDtoToGameCategory(CategoryDto dto)
    {
        return new()
        {
            Name = dto.Name
        };
    }

    private GameProduct ConvertDtoToGameProduct(GameDto dto)
    {
        GameProduct newGame = new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Developer = dto.Developer,
            Publisher = dto.Publisher,
            ReleaseDate = dto.ReleaseDate,
            Price = dto.Price,
            FileSize = dto.FileSize,
            SystemRequirements = dto.SystemRequirements
        };

        foreach(var category in dto.Categories)
        {
            newGame.Categories.Add(category);
        }

        return newGame;
    }
}

using EcommerceAPIDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPIDemo.Services;

public interface IGamesService
{
    //READ Operations
    public IQueryable<GameProduct>? GetAllGames(); //Pagination
    public IQueryable<GameCategory>? GetAllCategories(); //Pagination
    public Task<IQueryable<GameProduct>?> GetAllGamesInCategory(int categoryId); //Pagination
    public IQueryable<GameProduct>? GetAllGamesWithinPriceRange(double? minInclusive, double? maxExclusive); //Pagination
    public Task<GameProduct?> GetGame(int gameProductId);
    public Task<GameCategory?> GetCategory(int categoryId);

    //CREATE Operations
    public Task<GameProduct> CreateGame(GameDto dto);
    public Task<GameCategory> CreateCategory(CategoryDto dto);

    //UPDATE Operations
    public Task<GameProduct> UpdateGame(int gameProductId, GameDto dto);
    public Task<GameCategory> UpdateCategory(int gameCategoryId, CategoryDto dto);
}

public class GamesService : IGamesService
{
    private readonly SalesDbContext _salesDbContext;

    public GamesService(SalesDbContext salesDbContext)
    {
        _salesDbContext = salesDbContext;
    }

    public async Task<GameCategory> CreateCategory(CategoryDto dto)
    {
        GameCategory newCategory = ConvertDtoToGameCategory(dto);

        var savedCategory = _salesDbContext.GameCategories.Add(newCategory);
        await _salesDbContext.SaveChangesAsync();

        return savedCategory.Entity;
    }

    public async Task<GameProduct> CreateGame(GameDto dto)
    {
        GameProduct newGame = ConvertDtoToGameProduct(dto);

        var savedGame = _salesDbContext.GameProducts.Add(newGame);
        await _salesDbContext.SaveChangesAsync();

        return savedGame.Entity;
    }

    public IQueryable<GameCategory>? GetAllCategories()
    {
        var categories = _salesDbContext.GameCategories
            .OrderBy(c => c.Id);

        if(!categories.Any())
        {
            return null;
        }

        return categories;
    }

    public IQueryable<GameProduct>? GetAllGames()
    {
        var games = _salesDbContext.GameProducts
            .Include(p => p.Categories)
            .Include(p => p.Sales)
            .OrderBy(p => p.Id);

        if (!games.Any())
        {
            return null;
        }

        return games;
    }

    public async Task<IQueryable<GameProduct>?> GetAllGamesInCategory(int categoryId)
    {
        var category = await GetCategory(categoryId);
        var games = _salesDbContext.GameProducts.Where(product => product.Categories.Contains(category)).OrderBy(p => p.Id);

        if(!games.Any())
        {
            return null;
        }

        return games;
    }

    public IQueryable<GameProduct>? GetAllGamesWithinPriceRange(double? minInclusive, double? maxExclusive)
    {

        bool isMinValueNull = minInclusive == null;
        bool isMaxValueNull = maxExclusive == null;
        IQueryable<GameProduct> games;
        
        if(isMinValueNull) //Max Value Only
        {
            games = _salesDbContext.GameProducts.Where(game => game.Price < maxExclusive).OrderBy(p => p.Id);
        }
        else if(isMaxValueNull) //Min Value Only
        {
            games = _salesDbContext.GameProducts.Where(game => game.Price >= minInclusive).OrderBy(p => p.Id);
        }
        else //Both Bounds
        {
            games = _salesDbContext.GameProducts.Where(game => (game.Price < maxExclusive) && (game.Price >= minInclusive)).OrderBy(p => p.Id);
        }

        if(!games.Any())
        {
            return null;
        }
        
        return games;
    }

    public async Task<GameProduct?> GetGame(int gameProductId)
    {
        return await _salesDbContext.GameProducts
            .Include(p => p.Categories)
            .Include(p => p.Sales)
            .FirstOrDefaultAsync(p => p.Id == gameProductId);
    }

    public async Task<GameCategory?> GetCategory(int categoryId)
    {
        return await _salesDbContext.GameCategories.FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<GameCategory> UpdateCategory(int gameCategoryId, CategoryDto dto)
    {
        var existingCategory = await _salesDbContext.GameCategories
            .FirstOrDefaultAsync(c => c.Id == gameCategoryId);


        GameCategory newCategory = ConvertDtoToGameCategory(dto);
        newCategory.Id = existingCategory.Id;

        _salesDbContext.GameCategories.Entry(existingCategory).CurrentValues.SetValues(newCategory);
        await _salesDbContext.SaveChangesAsync();

        return existingCategory;
    }

    public async Task<GameProduct> UpdateGame(int gameProductId, GameDto dto)
    {
        var existingGame = await _salesDbContext.GameProducts
            .Include(p => p.Categories)
            .Include(p => p.Sales)
            .FirstOrDefaultAsync(p => p.Id == gameProductId);

        GameProduct newGame = ConvertDtoToGameProduct(dto);
        newGame.Id = existingGame.Id;

        _salesDbContext.GameProducts.Entry(existingGame).CurrentValues.SetValues(newGame);
        existingGame.Categories.Clear();
        existingGame.Categories.AddRange(newGame.Categories);
        await _salesDbContext.SaveChangesAsync();

        return existingGame;
    }

    private GameCategory ConvertDtoToGameCategory(CategoryDto dto)
    {
        GameCategory gameCategory = new()
        {
            Name = dto.Name  
        };

        return gameCategory;
    }

    private GameProduct ConvertDtoToGameProduct(GameDto dto)
    {
        GameProduct gameProduct = new()
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

        if (dto.GameCategoryIds != null)
        {
            foreach (var id in dto.GameCategoryIds)
            {
                var selectedCategory = _salesDbContext.GameCategories.Find(id);
                if (selectedCategory != null)
                {
                    _salesDbContext.Attach(selectedCategory);
                    gameProduct.Categories.Add(selectedCategory);
                }
            }
        }

        return gameProduct;
    }
}

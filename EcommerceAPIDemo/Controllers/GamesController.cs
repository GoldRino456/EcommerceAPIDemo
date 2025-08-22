using EcommerceAPIDemo.Data;
using EcommerceAPIDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPIDemo.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController :ControllerBase
{
    private readonly IGamesService _gamesService;
    public GamesController(IGamesService gamesService)
    {
        _gamesService = gamesService;
    }

    [HttpGet]
    public ActionResult<List<GameProduct>> GetAllGames()
    {
        var games = _gamesService.GetAllGames();

        if(games == null)
        {
            return NoContent();
        }

        return Ok(games);
    }

    [HttpGet("categories")]
    public ActionResult<List<GameCategory>> GetAllCategories()
    {
        var categories = _gamesService.GetAllCategories();

        if(categories == null)
        {
            return NoContent();
        }

        return Ok(categories);
    }

    [HttpGet("{gameId}")]
    public ActionResult<GameProduct> GetGame(int gameId)
    {
        var selectedGame = _gamesService.GetGame(gameId);

        if(selectedGame == null)
        {
            return NotFound($"Game with id {gameId} was not found.");
        }

        return Ok(selectedGame);
    }

    [HttpGet("categories/{categoryId}")]
    public ActionResult<GameCategory> GetCategory(int categoryId)
    {
        var selectedCategory = _gamesService.GetCategory(categoryId);

        if (selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        return Ok(selectedCategory);
    }

    [HttpGet("filter/categories/{categoryId}")]
    public ActionResult<List<GameProduct>> GetAllGamesInCategory(int categoryId)
    {
        var selectedCategory = _gamesService.GetCategory(categoryId);

        if(selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        var games = _gamesService.GetAllGamesInCategory(selectedCategory.Id);

        if(games == null)
        {
            return NoContent();
        }

        return Ok(games);
    }

    [HttpGet("filter/price")]
    public ActionResult<List<GameProduct>> GetAllGamesWithinPriceRange(double? minInclusive = null, double? maxExclusive = null)
    {
        if(minInclusive == null && maxExclusive == null)
        {
            return BadRequest("Missing values for Minimum and Maximum range. At least one value is needed to filter results.");
        }
        else if(minInclusive < 0 && maxExclusive <= 0)
        {
            return BadRequest("Invalid values for Minimum and Maximum range. Minimum must be zero or greater. Maximum must be greater than zero.");
        }
        else if(minInclusive == maxExclusive)
        {
            return BadRequest("Invalid values for Minimum and Maximum range. Minimum and Maximum values cannot be equal.");
        }

        var games = _gamesService.GetAllGamesWithinPriceRange(minInclusive, maxExclusive);

        if(games == null)
        {
            return NoContent();
        }

        return Ok(games);
    }

    [HttpPost]
    public ActionResult<GameProduct> CreateGame(GameDto dto)
    {
        return Ok(_gamesService.CreateGame(dto));
    }

    [HttpPost("categories")]
    public ActionResult<GameCategory> CreateCategory(CategoryDto dto)
    {
        return Ok(_gamesService.CreateCategory(dto));
    }

    [HttpPut("{gameId}")]
    public ActionResult<GameProduct> UpdateGame(int gameId, GameDto dto)
    {
        var selectedGame = _gamesService.GetGame(gameId);

        if(selectedGame == null)
        {
            return NotFound($"Game with id {gameId} was not found.");
        }

        return Ok(_gamesService.UpdateGame(selectedGame.Id, dto));
    }

    [HttpPut("categories/{categoryId}")]
    public ActionResult<GameCategory> UpdateCategory(int categoryId, CategoryDto dto)
    {
        var selectedCategory = _gamesService.GetCategory(categoryId);

        if (selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        return Ok(_gamesService.UpdateCategory(selectedCategory.Id, dto));
    }
}

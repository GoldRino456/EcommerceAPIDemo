using EcommerceAPIDemo.Data;
using EcommerceAPIDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<PagedResponse<GameProduct>>> GetAllGamesAsync([FromQuery] PaginationParams paginationParams)
    {
        var query = _gamesService.GetAllGames();

        if(query == null)
        {
            return NoContent();
        }

        var totalRecords = await query.CountAsync();
        var items = await query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        var pagedResponse = new PagedResponse<GameProduct>(items, paginationParams.PageNumber, paginationParams.PageSize, totalRecords);

        return Ok(pagedResponse);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<PagedResponse<GameCategory>>> GetAllCategoriesAsync([FromQuery] PaginationParams paginationParams)
    {
        var query = _gamesService.GetAllCategories();

        if (query == null)
        {
            return NoContent();
        }

        var totalRecords = await query.CountAsync();
        var items = await query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        var pagedResponse = new PagedResponse<GameCategory>(items, paginationParams.PageNumber, paginationParams.PageSize, totalRecords);

        return Ok(pagedResponse);
    }

    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameProduct>> GetGameAsync(int gameId)
    {
        var selectedGame = await _gamesService.GetGame(gameId);

        if(selectedGame == null)
        {
            return NotFound($"Game with id {gameId} was not found.");
        }

        return Ok(selectedGame);
    }

    [HttpGet("categories/{categoryId}")]
    public async Task<ActionResult<GameCategory>> GetCategoryAsync(int categoryId)
    {
        var selectedCategory = await _gamesService.GetCategory(categoryId);

        if (selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        return Ok(selectedCategory);
    }

    [HttpGet("filter/categories/{categoryId}")]
    public async Task<ActionResult<List<GameProduct>>> GetAllGamesInCategory(int categoryId)
    {
        var selectedCategory = await _gamesService.GetCategory(categoryId);

        if(selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        var games = await _gamesService.GetAllGamesInCategory(selectedCategory.Id);

        if(games == null)
        {
            return NoContent();
        }

        var gamesList = await games.ToListAsync();
        return Ok(gamesList);
    }

    [HttpGet("filter/price")]
    public async Task<ActionResult<List<GameProduct>>> GetAllGamesWithinPriceRange(double? minInclusive = null, double? maxExclusive = null)
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

        var gamesList = await games.ToListAsync();
        return Ok(gamesList);
    }

    [HttpPost]
    public async Task<ActionResult<GameProduct>> CreateGame(GameDto dto)
    {
        return Ok(await _gamesService.CreateGame(dto));
    }

    [HttpPost("categories")]
    public async Task<ActionResult<GameCategory>> CreateCategory(CategoryDto dto)
    {
        return Ok(await _gamesService.CreateCategory(dto));
    }

    [HttpPut("{gameId}")]
    public async Task<ActionResult<GameProduct>> UpdateGame(int gameId, GameDto dto)
    {
        var selectedGame = await _gamesService.GetGame(gameId);

        if(selectedGame == null)
        {
            return NotFound($"Game with id {gameId} was not found.");
        }

        return Ok(_gamesService.UpdateGame(selectedGame.Id, dto));
    }

    [HttpPut("categories/{categoryId}")]
    public async Task<ActionResult<GameCategory>> UpdateCategory(int categoryId, CategoryDto dto)
    {
        var selectedCategory = await _gamesService.GetCategory(categoryId);

        if (selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        return Ok(_gamesService.UpdateCategory(selectedCategory.Id, dto));
    }
}

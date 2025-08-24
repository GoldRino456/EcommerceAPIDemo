using EcommerceAPIDemo.Data;
using EcommerceAPIDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
    public async Task<ActionResult<List<GameProduct>>> GetAllGamesInCategoryAsync(int categoryId, [FromQuery] PaginationParams paginationParams)
    {
        var selectedCategory = await _gamesService.GetCategory(categoryId);

        if(selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        var query = await _gamesService.GetAllGamesInCategory(selectedCategory.Id);

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

    [HttpGet("filter/price")]
    public async Task<ActionResult<List<GameProduct>>> GetAllGamesWithinPriceRangeAsync([FromQuery] PaginationParams paginationParams, double? minInclusive = null, double? maxExclusive = null)
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

        var query = _gamesService.GetAllGamesWithinPriceRange(minInclusive, maxExclusive);

        if (query == null)
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

    [HttpPost]
    public async Task<ActionResult<GameProduct>> CreateGameAsync(GameDto dto)
    {
        return Ok(await _gamesService.CreateGame(dto));
    }

    [HttpPost("categories")]
    public async Task<ActionResult<GameCategory>> CreateCategoryAsync(CategoryDto dto)
    {
        return Ok(await _gamesService.CreateCategory(dto));
    }

    [HttpPut("{gameId}")]
    public async Task<ActionResult<GameProduct>> UpdateGameAsync(int gameId, GameDto dto)
    {
        var selectedGame = await _gamesService.GetGame(gameId);

        if(selectedGame == null)
        {
            return NotFound($"Game with id {gameId} was not found.");
        }

        return Ok(await _gamesService.UpdateGame(selectedGame.Id, dto));
    }

    [HttpPut("categories/{categoryId}")]
    public async Task<ActionResult<GameCategory>> UpdateCategoryAsync(int categoryId, CategoryDto dto)
    {
        var selectedCategory = await _gamesService.GetCategory(categoryId);

        if (selectedCategory == null)
        {
            return NotFound($"Category with id {categoryId} was not found.");
        }

        return Ok(await _gamesService.UpdateCategory(selectedCategory.Id, dto));
    }
}

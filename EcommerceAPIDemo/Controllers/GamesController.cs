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
    public async Task<ActionResult<PagedResponse<GameProduct>>> GetAllGamesAsync([FromQuery] GameProductQuery gameProductQuery)
    {
        var query = _gamesService.GetAllGames();

        if (query == null)
        {
            return NoContent();
        }

        if (gameProductQuery.GameProductFilters.CategoryId.HasValue)
        {
            query = query.Where(p => p.Categories.Any(c => c.Id == gameProductQuery.GameProductFilters.CategoryId.Value));
        }
        if(gameProductQuery.GameProductFilters.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= gameProductQuery.GameProductFilters.MinPrice.Value);
        }
        if (gameProductQuery.GameProductFilters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price < gameProductQuery.GameProductFilters.MaxPrice.Value);
        }


        var totalRecords = await query.CountAsync();
        var items = await query.Skip((gameProductQuery.Pagination.PageNumber - 1) * gameProductQuery.Pagination.PageSize)
            .Take(gameProductQuery.Pagination.PageSize)
            .ToListAsync();

        var pagedResponse = new PagedResponse<GameProduct>(items, gameProductQuery.Pagination.PageNumber, gameProductQuery.Pagination.PageSize, totalRecords);

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

    [HttpPost]
    public async Task<ActionResult<GameProduct>> CreateGameAsync(NewGameDto dto)
    {
        return Ok(await _gamesService.CreateGame(dto));
    }

    [HttpPost("categories")]
    public async Task<ActionResult<GameCategory>> CreateCategoryAsync(CategoryDto dto)
    {
        return Ok(await _gamesService.CreateCategory(dto));
    }

    [HttpPut("{gameId}")]
    public async Task<ActionResult<GameProduct>> UpdateGameAsync(int gameId, ExistingGameDto dto)
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

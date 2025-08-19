using EcommerceAPIDemo.Data;
using EcommerceAPIDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPIDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpGet]
    public ActionResult<List<GameCategory>> GetAllCategories()
    {
        var categories = _gamesService.GetAllCategories();

        if(categories == null)
        {
            return NoContent();
        }

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public ActionResult<GameProduct> GetGame(int id)
    {
        var selectedGame = _gamesService.GetGame(id);

        if(selectedGame == null)
        {
            return NotFound();
        }

        return Ok(selectedGame);
    }

    [HttpGet("categories/{id}")]
    public ActionResult<GameCategory> GetCategory(int id)
    {
        var selectedCategory = _gamesService.GetCategory(id);

        if (selectedCategory == null)
        {
            return NotFound();
        }

        return Ok(selectedCategory);
    }
}

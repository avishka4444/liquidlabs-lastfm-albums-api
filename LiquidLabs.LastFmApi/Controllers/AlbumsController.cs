using LiquidLabs.LastFmApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiquidLabs.LastFmApi.Controllers;

[ApiController]
[Route("albums")]
public sealed class AlbumsController : ControllerBase
{
    private readonly IAlbumService _service;

    public AlbumsController(IAlbumService service)
    {
        _service = service;
    }

    // GET all the top albums for The Beatles
    [HttpGet]
    public async Task<IActionResult> GetTopAlbums()
    {
        var albums = await _service.GetTopAlbumsAsync();
        return Ok(albums);
    }

    // GET an album by ID - /albums/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAlbumById([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "id must be a positive integer" });

        var album = await _service.GetAlbumByIdAsync(id);
        if (album is null) return NotFound();

        return Ok(album);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraSmartAPI.Data;
using LibraSmartAPI.Models;

namespace LibraSmartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrariesController : ControllerBase
{
    private readonly LibraryContext _context;

    public LibrariesController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Library>>> GetLibraries()
    {
        return await _context.Libraries.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Library>> GetLibrary(int id)
    {
        var library = await _context.Libraries.FindAsync(id);

        if (library == null)
        {
            return NotFound();
        }

        return library;
    }
}

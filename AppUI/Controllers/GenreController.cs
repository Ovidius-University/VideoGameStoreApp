using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
namespace AppUI.Controllers;
public class GenreController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GenreController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Genres")]
    public async Task<IActionResult> Index()
    {
        var genres = await _context.Genres
            .OrderBy(i => i.Name)
            .ToListAsync();
        return View(_mapper.Map<List<CardGenreVm>>(genres));
    }

    [Route("Genre/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Genre = await _context.Genres
            .FirstOrDefaultAsync(i => i.Id == id);
        if (Genre == null)
            return NotFound();
        var VideoGames = await _context.VideoGames
            .Include(t => t.Publisher)
            .Include(t => t.Offer)
            .Include(t => t.Genre)
            .Where(i => i.GenreId! == id && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();
        var output = new GenreVideoGamesVm()
        {
            GenreDetails = _mapper.Map<CardGenreVm>(Genre),
            VideoGames = _mapper.Map<List<CardVideoGameVm>>(VideoGames)
        };
        return View(output);
    }
}
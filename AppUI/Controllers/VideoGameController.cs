using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
namespace AppUI.Controllers;
public class VideoGameController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public VideoGameController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("VideoGames")]
    public async Task<IActionResult> Index()
    {
        var VideoGames = await _context.VideoGames
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Publisher)
            .Include(t => t.Genre)
            .Include(t => t.Offer)
            .Where(i => i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();
        return View(_mapper.Map<List<CardVideoGameVm>>(VideoGames));
    }

    [Route("VideoGame/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var VideoGame = _mapper.Map<VideoGameDetailsVm>(await _context.VideoGames
            .Include(t => t.Publisher)
            .Include(t => t.Offer)
            .Include(t => t.Genre)
            .Where(i => i.IsFinal)
            .FirstOrDefaultAsync(i => i.Id == id));
        if (VideoGame == null)
            return NotFound();
        var Developers = _mapper.Map<List<CardDeveloperVm>>(await _context.Developers
            .Where(i => i.VideoGames!.Any(i => i.VideoGameId == id))
            .ToListAsync());
        VideoGame.ListDevelopers = Developers;
        return View(VideoGame);
    }

    [Route("Offers")]
    public async Task<IActionResult> Offers()
    {
        var VideoGames = await _context.VideoGames
            .Include(t => t.Developers!)
                .ThenInclude(t => t.Developer)
            .Include(t => t.Publisher)
            .Include(t => t.Offer)
            .Where(i => i.IsFinal && i.Offer != null)
            .OrderBy(i => i.Title)
            .ToListAsync();
        return View(_mapper.Map<List<CardVideoGameVm>>(VideoGames));
    }
}
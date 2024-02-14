using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
namespace AppUI.Controllers;
public class DeveloperController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public DeveloperController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Developers")]
    public async Task<IActionResult> Index()
    {
        var Developers = await _context.Developers
            .OrderBy(i => i.LastName)
            .ThenBy(i => i.FirstName)
            .ToListAsync();
        return View(_mapper.Map<List<CardDeveloperVm>>(Developers));
    }

    [Route("Developer/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Developer = await _context.Developers
            .FirstOrDefaultAsync(i => i.Id == id);
        if (Developer == null)
            return NotFound();
        var VideoGames = await _context.VideoGames
            .Include(t => t.Publisher)
            .Include(t => t.Offer)
            .Where(i => i.Developers!.Any(i => i.DeveloperId == id) && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();
        var output = new DeveloperVideoGamesVm()
        {
            DeveloperDetails = _mapper.Map<CardDeveloperVm>(Developer),
            VideoGames = _mapper.Map<List<CardVideoGameVm>>(VideoGames)
        };
        return View(output);
    }
}
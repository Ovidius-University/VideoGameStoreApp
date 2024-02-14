using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
namespace AppUI.Controllers;
public class SuggestionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public SuggestionController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Suggestions")]
    public async Task<IActionResult> Index()
    {
        var CurrentCustomer = await _context.Users
                .FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("Identity/Account/Login");

        var OwnedOrders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Where(q => q.CustomerId == CurrentCustomer.Id)
            .ToListAsync();
        if (OwnedOrders.Count == 0 || OwnedOrders == null)
        {
            TempData["MessageShopCart"] = $"In order to get an order suggestion, {CurrentCustomer.UserName}, you need to make an order for us to match it with other people's orders!";
            return RedirectToAction("Index", "ShopCart");
        }
        var OwnedOrderContents = await _context.OrderContents
            .Include(c => c.VideoGame)
            .ToListAsync();

        OwnedOrderContents = OwnedOrderContents
            .Where(q => OwnedOrders.Any(j => j.Id == q.OrderId))
            .ToList();

        var OwnedVideoGames = await _context.VideoGames
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Publisher)
            .Include(t => t.Genre)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        OwnedVideoGames = OwnedVideoGames
            .Where(i => OwnedOrderContents.Any(j => j.VideoGameId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();

        var DifferentOrders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Where(q => q.CustomerId != CurrentCustomer.Id)
            .ToListAsync();
        var DifferentUsers = DifferentOrders
            .Where(q => q.OrderContents!.Any(p => OwnedOrderContents.Any(j => j.VideoGameId == p.VideoGameId)))
            .ToList();
        DifferentOrders = DifferentOrders
            .Where(q => DifferentUsers.Any(j => j.CustomerId == q.CustomerId))
            .ToList();
        if (DifferentOrders.Count == 0 || DifferentOrders == null)
        {
            TempData["MessageHome"] = $"In order to get an order suggestion, {CurrentCustomer.UserName}, other people need to make an order with some of the video games you have purchased for us to show you what else they bought!";
            return RedirectToAction("Index", "Home");
        }
        var DifferentOrderContents = await _context.OrderContents
            .Include(c => c.VideoGame)
            .ToListAsync();

        DifferentOrderContents = DifferentOrderContents
            .Where(q => DifferentOrders.Any(j => j.Id == q.OrderId))
            .ToList();

        var DifferentVideoGames = await _context.VideoGames
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Publisher)
            .Include(t => t.Genre)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        DifferentVideoGames = DifferentVideoGames
            .Where(i => DifferentOrderContents.Any(j => j.VideoGameId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();

        var Suggestions = await _context.VideoGames
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Publisher)
            .Include(t => t.Genre)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        Suggestions = Suggestions
            .Where(i => !OwnedVideoGames.Any(j => j.Id == i.Id) && DifferentVideoGames.Any(j => j.Id == i.Id) && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToList();
        return View(_mapper.Map<List<CardVideoGameVm>>(Suggestions));
    }
    /*
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
    */
}
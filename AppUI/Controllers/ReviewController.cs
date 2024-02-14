using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
namespace AppUI.Controllers;
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ReviewController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Reviews")]
    public async Task<IActionResult> Index()
    {
        var Reviews = await _context.Reviews
            .Include(t => t.Reviewer!)
            .Include(t => t.VideoGame!)
            .Where(i => i.IsFinal && i.VideoGame!.IsFinal)
            .OrderBy(i => i.VideoGameId)
            .ToListAsync();
        return View(_mapper.Map<List<CardReviewVm>>(Reviews));
    }

    [ActionName("Details")]
    public async Task<IActionResult> Details(int? vid, int? rid)
    {
        var Review = _mapper.Map<ReviewDetailsVm>(await _context.Reviews
            .Include(i => i.VideoGame)
            .Include(i => i.Reviewer)
            .Where(i => i.IsFinal)
            .FirstOrDefaultAsync(i => i.VideoGameId == vid && i.ReviewerId == rid));
        if (Review == null)
            return NotFound();
        /*
        var Reviewers = _mapper.Map<CardReviewerVm>(await _context.Reviewers
            .Where(i => i.Reviews!.Any(i => i.VideoGameId == vid && i.ReviewerId == rid))
            .FirstOrDefaultAsync());
        Review.Reviewer = Reviewers.Name;
        */
        return View(Review);
    }
}
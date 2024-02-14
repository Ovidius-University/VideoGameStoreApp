using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Models.Entities;
namespace AppUI.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Roles = "Manager")]
public class DeveloperController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeveloperController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: Manager/Developer
    [Route("Manager/Developers")]
    public async Task<IActionResult> Index()
    {
        if (_context.Developers == null)
            return Problem("Entity set 'ApplicationDbContext.Developers' is null.");
        var CurrentPublisher = await _context.PublisherManagers
                .Include(t => t.Publisher)
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentPublisher == null)
            return Problem("You haven't been assigned to a publisher yet!");
        var Developers = await _context.Developers.ToListAsync();
        //List<ShortDeveloperVm> lista = new();
        //foreach(var developer in Developers)
        //{
        //    lista.Add(new ShortDeveloperVm(developer.Id, developer.FirstName, developer.LastName));
        //}
        //return View(lista);
        return  View(_mapper.Map<List<ShortDeveloperVm>>(Developers));
    }

    // GET: Manager/Developer/Details/5
    [Route("Manager/Developer/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Developers == null)
        {
            return NotFound();
        }

        var developer = await _context.Developers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (developer == null)
        {
            return NotFound();
        }

        return View(_mapper.Map<DeveloperDetailsVm>(developer));
    }

    // GET: Manager/Developer/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Manager/Developer/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm]NewDeveloperDto NewDeveloper)
    {
        if (ModelState.IsValid)
        {
            //Developer developer = new()
            //{
            //    FirstName = NewDeveloper.FirstName,
            //    LastName = NewDeveloper.LastName,
            //    Birthday = NewDeveloper.Birthday
            //};
            //_context.Add(developer);
            var Developer = _mapper.Map<Developer>(NewDeveloper);
            _context.Add(Developer);
            await _context.SaveChangesAsync();
            TempData["MessageDeveloper"] = $"The developer <strong>{NewDeveloper.FirstName} {NewDeveloper.LastName}</strong> was successfully added!";
            return RedirectToAction(nameof(Index));
        }
        return View(NewDeveloper);
    }

    // GET: Manager/Developer/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        if (_context.Developers == null)
        {
            return NotFound();
        }

        var existentDeveloper = _mapper.Map<ExistentDeveloperDto>(await _context.Developers.FindAsync(id));
        if (existentDeveloper == null)
        {
            return NotFound();
        }
        return View(existentDeveloper);
    }

    // POST: Manager/Developer/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm]ExistentDeveloperDto existentDeveloper)
    {
        var developer = await _context.Developers.FindAsync(id);
        if (id != existentDeveloper.DeveloperId || id!=developer!.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageDeveloper"] = $"The developer <strong>{developer.FirstName} {developer.LastName}</strong> was modified to <strong>{existentDeveloper.FirstName} {existentDeveloper.LastName}</strong> successfully!";
                existentDeveloper.ToEntity(ref developer);
                _context.Update(developer);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeveloperExists(existentDeveloper.DeveloperId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(existentDeveloper);
    }

    // GET: Manager/Developer/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Developers == null)
        {
            return NotFound();
        }

        var developer = await _context.Developers
            .FirstOrDefaultAsync(m => m.Id == id);
        if (developer == null)
        {
            return NotFound();
        }

        var DeveloperInVideoGames = await _context.DevelopersVideoGame.Where(q => q.DeveloperId == id).CountAsync();

        if (DeveloperInVideoGames > 0)
        {
            TempData["MessageDeveloper"] = $"<strong>{developer.FirstName} {developer.LastName}</strong> cannot be deleted as they are a developer for video games in the store, to be more exact: {DeveloperInVideoGames}!";
            return RedirectToAction(nameof(Index));
        }
        
        return View(_mapper.Map<ShortDeveloperVm>(developer));
    }

    // POST: Manager/Developer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Developers == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Developers' is null.");
        }
        var existentDeveloper = await _context.Developers.FindAsync(id);
        if (existentDeveloper is null)
            return NotFound();
        var DeveloperInVideoGames = await _context.DevelopersVideoGame.Where(q=>q.DeveloperId == id).CountAsync();
        if (DeveloperInVideoGames==0)
        {
            _context.Developers.Remove(existentDeveloper);
            TempData["MessageDeveloper"] = $"The developer <strong>{existentDeveloper.FirstName} {existentDeveloper.LastName}</strong> was deleted successfully!";
            await _context.SaveChangesAsync();
        }
        else
        {
            TempData["MessageDeveloper"] = $"<strong>{existentDeveloper.FirstName} {existentDeveloper.LastName}</strong> was not deleted as they are a developer for video games in the store, to be more exact: {DeveloperInVideoGames}!";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Manager/Developer/AddVideoGame/5
    public async Task<IActionResult> AddVideoGame(int id)
    {
        if (_context.Developers == null)
        {
            return NotFound();
        }

        var existentDeveloper = _mapper.Map<DeveloperAddVideoGameDto>(await _context.Developers.FindAsync(id));
        if (existentDeveloper == null)
        {
            return NotFound();
        }
        var CurrentPublisher = await _context.PublisherManagers
                .Include(t => t.Publisher)
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentPublisher == null)
            return NotFound();//redirectionez
        var videoGames = _mapper.Map<List<VideoGameDto>>(
            await _context.VideoGames
                .Include(t => t.Developers)
                .Where(q => q.PublisherId == CurrentPublisher.PublisherId && !q.IsFinal).ToListAsync());
        ViewBag.AvailableVideoGames = new SelectList(videoGames, "VideoGameId", "Title");
        return View(existentDeveloper);
    }


    // POST: Manager/Developer/AddVideoGame/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVideoGame(int id, [FromForm] DeveloperAddVideoGameDto DeveloperAddVideoGame)
    {
        var existentDeveloper = await _context.Developers.FindAsync(id);
        var ExistentVideoGame = await _context.VideoGames.FindAsync(DeveloperAddVideoGame.VideoGameId);
        if(existentDeveloper is null || ExistentVideoGame is null)
            return NotFound();
        if (id != DeveloperAddVideoGame.DeveloperId || id != existentDeveloper!.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existentDeveloperVideoGame = await _context.DevelopersVideoGame.FirstOrDefaultAsync(q => q.DeveloperId == DeveloperAddVideoGame.DeveloperId && q.VideoGameId == DeveloperAddVideoGame.VideoGameId);
                if(existentDeveloperVideoGame is null)
                {
                    DeveloperVideoGame DeveloperVideoGame = new()
                    {
                        DeveloperId = DeveloperAddVideoGame.DeveloperId,
                        VideoGameId = DeveloperAddVideoGame.VideoGameId
                    };
                    await _context.DevelopersVideoGame.AddAsync(DeveloperVideoGame);
                    await _context.SaveChangesAsync();
                    TempData["MessageDeveloper"] = $"The developer <strong>{existentDeveloper.FirstName} {existentDeveloper.LastName}</strong> was successfully associated with video game <strong>{ExistentVideoGame.Title}</strong>!";
                }
                else
                {
                    TempData["MessageDeveloper"] = $"The developer <strong>{existentDeveloper.FirstName} {existentDeveloper.LastName}</strong> was already associated with video game <strong>{ExistentVideoGame.Title}</strong>!";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeveloperVideoGameExists(DeveloperAddVideoGame.DeveloperId, DeveloperAddVideoGame.VideoGameId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(DeveloperAddVideoGame);
    }

    // GET: Manager/Developer/RemoveVideoGame/5
    public async Task<IActionResult> RemoveVideoGame(int id)
    {
        if (_context.Developers == null)
        {
            return NotFound();
        }

        var existentDeveloper = _mapper.Map<DeveloperAddVideoGameDto>(await _context.Developers.FindAsync(id));
        if (existentDeveloper == null)
        {
            return NotFound();
        }
        var CurrentPublisher = await _context.PublisherManagers
                .Include(t => t.Publisher)
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
        if (CurrentPublisher == null)
            return NotFound();//redirectionez
        var videoGames = _mapper.Map<List<VideoGameDto>>(
            await _context.VideoGames
                .Include(t => t.Developers)
                .Where(q => q.PublisherId == CurrentPublisher.PublisherId && !q.IsFinal).ToListAsync());
        ViewBag.AcquiredVideoGames = new SelectList(videoGames, "VideoGameId", "Title");
        return View(existentDeveloper);
    }


    // POST: Manager/Developer/RemoveVideoGame/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveVideoGame(int id, [FromForm] DeveloperAddVideoGameDto DeveloperRemoveVideoGame)
    {
        var existentDeveloper = await _context.Developers.FindAsync(id);
        var ExistentVideoGame = await _context.VideoGames.FindAsync(DeveloperRemoveVideoGame.VideoGameId);
        if (existentDeveloper is null || ExistentVideoGame is null)
            return NotFound();
        if (id != DeveloperRemoveVideoGame.DeveloperId || id != existentDeveloper!.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var existentDeveloperVideoGame = await _context.DevelopersVideoGame.FirstOrDefaultAsync(q => q.DeveloperId == DeveloperRemoveVideoGame.DeveloperId && q.VideoGameId == DeveloperRemoveVideoGame.VideoGameId);
                if (existentDeveloperVideoGame is not null)
                {
                    _context.DevelopersVideoGame.Remove(existentDeveloperVideoGame);
                    await _context.SaveChangesAsync();
                    TempData["MessageDeveloper"] = $"The developer <strong>{existentDeveloper.FirstName} {existentDeveloper.LastName}</strong> was successfully disassociated with video game <strong>{ExistentVideoGame.Title}</strong>!";
                }
                else
                {
                    TempData["MessageDeveloper"] = $"The developer <strong>{existentDeveloper.FirstName} {existentDeveloper.LastName}</strong> was already not associated with video game <strong>{ExistentVideoGame.Title}</strong>!";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeveloperVideoGameExists(DeveloperRemoveVideoGame.DeveloperId, DeveloperRemoveVideoGame.VideoGameId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(DeveloperRemoveVideoGame);
    }

    private bool DeveloperExists(int DeveloperId)
    {
        return (_context.Developers?.Any(e => e.Id == DeveloperId)).GetValueOrDefault();
    }

    private bool DeveloperVideoGameExists(int DeveloperId, int VideoGameId)
    {
      return (_context.DevelopersVideoGame?.Any(e => e.DeveloperId == DeveloperId && e.VideoGameId == VideoGameId)).GetValueOrDefault();
    }
}

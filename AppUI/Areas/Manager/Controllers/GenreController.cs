using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
namespace AppUI.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize(Roles = "Manager")]
public class GenreController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public GenreController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Manager/Genre
    [Route("Manager/Genres")]
    public async Task<IActionResult> Index()
    {
        if (_context.Genres == null)
            return Problem("Entity set 'ApplicationDbContext.Genres' is null.");
        return View(_mapper.Map<List<ExistentGenreDto>>(await _context.Genres.ToListAsync()));
    }

    // GET: Manager/Genre/Details/5
    [Route("Manager/Genre/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Genres == null)
        {
            return NotFound();
        }

        var ExistentGenre = _mapper.Map<ExistentGenreDto>(await _context.Genres.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentGenre == null)
        {
            return NotFound();
        }

        return View(ExistentGenre);
    }

    // GET: Manager/Genre/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Manager/Genre/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewGenreDto newGenre)
    {
        if (ModelState.IsValid)
        {
            var history = await _context.Genres
                .Where(i => i.Name == newGenre.Name)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageGenre"] = $"The genre <strong>{newGenre.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }
            var genre = _mapper.Map<Genre>(newGenre);
            _context.Add(genre);
            await _context.SaveChangesAsync();
            TempData["MessageGenre"] = $"We added genre <strong>{newGenre.Name}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newGenre);
    }

    // GET: Manager/Genre/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Genres == null)
        {
            return NotFound();
        }

        var ExistentGenre = _mapper.Map<ExistentGenreDto>(await _context.Genres.FindAsync(id));
        if (ExistentGenre == null)
        {
            return NotFound();
        }
        return View(ExistentGenre);
    }

    // POST: Manager/Genre/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentGenreDto modifiedGenre)
    {
        if (id != modifiedGenre.Id)
        {
            return NotFound();
        }
        var ExistentGenre = await _context.Genres.FindAsync(id);
        if (ExistentGenre == null)
        {
            return NotFound();
        }
        if (ExistentGenre.Id != modifiedGenre.Id)
        {
            return BadRequest();
        }

        var history = await _context.Genres
                .Where(i => i.Name == modifiedGenre.Name && i.Id!=id)
                .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessageGenre"] = $"The genre <strong>{modifiedGenre.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }
        if (ModelState.IsValid)
        {
            try
            {
                TempData["MessageGenre"] = $"We modified genre <strong>{ExistentGenre.Name}</strong> in <strong>{modifiedGenre.Name}</strong>!";
                ExistentGenre.Name = modifiedGenre.Name;
                _context.Update(ExistentGenre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(ExistentGenre.Id))
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
        return View(modifiedGenre);
    }

    // GET: Manager/Genre/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Genres == null)
        {
            return NotFound();
        }

        var ExistentGenre = _mapper.Map<ExistentGenreDto>(await _context.Genres.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentGenre == null)
        {
            return NotFound();
        }

        var VideoGameGenre = await _context.Genres
            .Include(t => t.VideoGames)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (VideoGameGenre == null)
        {
            return NotFound();
        }

        if (VideoGameGenre.VideoGames!.Count > 0)
        {
            TempData["MessageGenre"] = $"Genre <strong>{ExistentGenre.Name}</strong> can not be deleted as it has associated video games, {VideoGameGenre.VideoGames!.Count} to be more exact!";
            return RedirectToAction(nameof(Index));
        }   

        return View(ExistentGenre);
    }

    // POST: Manager/Genre/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Genres == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Genres' is null.");
        }
        var ExistentGenre = await _context.Genres
            .Include(t => t.VideoGames)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentGenre == null)
        {
            return NotFound();
        }
        else
        {
            if (ExistentGenre.VideoGames!.Count == 0)
            {
                _context.Genres.Remove(ExistentGenre);
                TempData["MessageGenre"] = $"Genre <strong>{ExistentGenre.Name}</strong> was successfully deleted!";
            }
            else
            {
                TempData["MessageGenre"] = $"Genre <strong>{ExistentGenre.Name}</strong> can not be deleted as it has associated video games!";
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool GenreExists(int id)
    {
        return (_context.Genres?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
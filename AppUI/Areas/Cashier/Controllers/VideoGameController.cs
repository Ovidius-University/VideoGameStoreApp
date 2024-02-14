using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AppUI.Areas.Cashier.Models.DTOs;
using AppUI.Areas.Cashier.Models.ViewModels;
using AppUI.Data;
using AppUI.Models.CustomIdentity;
namespace AppUI.Areas.Cashier.Controllers
{
    [Area("Cashier")]
    [Authorize(Roles = "Cashier")]
    public class VideoGameController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public VideoGameController(ApplicationDbContext context, IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: Cashier/VideoGame
        [Route("Cashier/VideoGames")]
        public async Task<IActionResult> Index()
        {
            var videoGames = _mapper.Map<List<VideoGameVm>>(
                await _context.VideoGames
                .Include(t => t.Offer)
                .Include(t => t.Genre)
                .Include(t => t.Publisher)
                .Where(t => t.IsFinal == true)
                .OrderBy(t => t.Title)
                .ToListAsync());
            var output = new IndexVideoGamesVm()
            {
                ListVideoGames = videoGames
            };
            return View(output);
        }

        // GET: Cashier/VideoGame/Details/5
        [Route("Cashier/VideoGame/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VideoGames == null)
            {
                return NotFound();
            }

            var videoGame = await _context.VideoGames
                .Include(t => t.Offer)
                .Include(c => c.Publisher)
                .Include(c => c.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videoGame == null)
            {
                return NotFound();
            }
            var ListDevelopers = await _context.DevelopersVideoGame
                .Include(t => t.Developer)
                .Where(c => c.VideoGameId == id)
                .OrderBy(o => o.Developer!.LastName)
                .ThenBy(o => o.Developer!.FirstName)
                .ToListAsync();

            var output = _mapper.Map<VideoGameDetailsVm>(videoGame);
            var developers = "";
            foreach (var developer in ListDevelopers)
            {
                developers += $"{developer.Developer!.FirstName} {developer.Developer!.LastName}, ";
            }
            output.Developers = string.IsNullOrEmpty(developers) ? "-" : developers.Substring(0, developers.Length - 2);
            output.Genre = videoGame.Genre!.Name;
            return View(output);
        }

        // GET: Cashier/VideoGame/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VideoGames == null)
            {
                return NotFound();
            }
            var ExistentVideoGame = await _context.VideoGames
                .Include(t => t.Offer)
                .Include(c => c.Genre)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (ExistentVideoGame == null)
            {
                return NotFound();
            }
            var developers = await _context.DevelopersVideoGame
                .Include(t => t.Developer).Where(c => c.VideoGameId == id)
                .OrderBy(o => o.Developer!.LastName)
                .ThenBy(o => o.Developer!.FirstName)
                .ToListAsync();
            var output = new VideoGameEditDto()
            {
                ExistentVideoGame = _mapper.Map<ExistentVideoGameDto>(ExistentVideoGame),
                ListDevelopers = _mapper.Map<List<ShortDeveloperVm>>(developers)
            };
            return View(output);
        }

        // POST: Cashier/VideoGame/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] VideoGameEditDto ModifiedVideoGame)
        {
            if (ModifiedVideoGame.ExistentVideoGame is null || id != ModifiedVideoGame.ExistentVideoGame!.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var ExistentVideoGame = await _context.VideoGames
                        .FirstOrDefaultAsync(q => q.Id == id);
                    if (ExistentVideoGame == null)
                    {
                        return NotFound();
                    }
                    ModifiedVideoGame.ExistentVideoGame.ToEntity(ref ExistentVideoGame);
                    _context.Update(ExistentVideoGame);
                    await _context.SaveChangesAsync();

                    if (ExistentVideoGame!.Stock == 0)
                    {
                        var unavailableshopcarts = await _context.ShopCarts
                        .Include(c => c.VideoGame)
                        .Include(c => c.VideoGame!.Offer)
                        .Where(q => q.VideoGameId == id).ToListAsync();

                        foreach (var item in unavailableshopcarts)
                        {
                            _context.ShopCarts.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                    }

                    var shopcarts = await _context.ShopCarts
                        .Include(c => c.VideoGame)
                        .Include(c => c.VideoGame!.Offer)
                        .Where(q => q.VideoGameId == id && q.VideoGame!.Stock < q.Quantity)
                        .ToListAsync();
                    foreach (var item in shopcarts)
                    {
                        item.Quantity = ExistentVideoGame.Stock;
                        _context.ShopCarts.Update(item);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoGameExists(ModifiedVideoGame.ExistentVideoGame.Id))
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
            var developers = await _context.DevelopersVideoGame
                .Include(t => t.Developer)
                .Where(c => c.VideoGameId == id)
                .ToListAsync();
            ModifiedVideoGame.ListDevelopers = _mapper.Map<List<ShortDeveloperVm>>(developers);
            return View(ModifiedVideoGame);
        }

        private bool VideoGameExists(int id)
        {
            return (_context.VideoGames?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

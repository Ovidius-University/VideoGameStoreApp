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
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Data;
using AppUI.Models.CustomIdentity;
namespace AppUI.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
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

        // GET: Manager/VideoGame
        [Route("Manager/VideoGames")]
        public async Task<IActionResult> Index()
        {
            var CurrentPublisher = await _context.PublisherManagers
                .Include(t => t.Publisher)
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);
            if (CurrentPublisher == null)
                return Problem("You haven't been assigned to a publisher yet!");
            var videoGames = _mapper.Map<List<VideoGameVm>>(
                await _context.VideoGames
                .Include(t => t.Genre)
                .Where(q => q.PublisherId == CurrentPublisher.PublisherId).ToListAsync());
            var output = new IndexVideoGamesVm()
            {
                Publisher = CurrentPublisher.Publisher!.Name,
                ListVideoGames = videoGames
            };
            return View(output);
        }

        // GET: Manager/VideoGame/Details/5
        [Route("Manager/VideoGame/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.VideoGames == null)
            {
                return NotFound();
            }

            var videoGame = await _context.VideoGames
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

        // GET: Manager/VideoGame/Create
        public async Task<IActionResult> Create()
        {
            if (_context.Genres == null || _context.VideoGames == null)
            {
                return NotFound();
            }

            var CurrentManager = await _context.PublisherManagers
                .Include(t => t.Publisher)
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(q => q.Manager!.UserName == User.Identity!.Name);

            if (CurrentManager == null)
                return NotFound();//redirectionez
            var genres = _mapper.Map<List<GenreDto>>(
                await _context.Genres
                .ToListAsync());
            if(genres == null)
            {
                return NotFound();
            }
            ViewBag.AvailableGenres = new SelectList(genres, "GenreId", "Name");
            return View();
        }

        // POST: Manager/VideoGame/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] NewVideoGameDto newVideoGame)
        {
            var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            var CurrentPublisher = await _context.PublisherManagers.Where(q => q.ManagerId == CurrentUser!.Id).FirstOrDefaultAsync();
            if (CurrentPublisher == null)
                return BadRequest();
            var CurrentGenre = await _context.Genres
                .Where(q => q.Id == newVideoGame!.GenreId).FirstOrDefaultAsync();
            if (CurrentGenre == null)
                return BadRequest();
            if (ModelState.IsValid)
            {
                var history = await _context.VideoGames
                .Where(i => i.Title == newVideoGame.Title)
                .FirstOrDefaultAsync();
                if (history != null)
                {
                    TempData["MessageVideoGame"] = $"The video game <strong>{newVideoGame.Title}</strong> is already added!";
                    return RedirectToAction(nameof(Index));
                }
                var NewVideoGame = _mapper.Map<VideoGame>(newVideoGame);
                NewVideoGame.PublisherId = CurrentPublisher!.PublisherId;
                _context.Add(NewVideoGame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var genres = _mapper.Map<List<GenreDto>>(
                await _context.Genres
                .ToListAsync());
            if (genres == null)
            {
                return NotFound();
            }
            ViewBag.AvailableGenres = new SelectList(genres, "GenreId", "Name");
            return View(newVideoGame);
        }

        // GET: Manager/VideoGame/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.VideoGames == null)
            {
                return NotFound();
            }
            var CurrentPublisher = await _context.PublisherManagers
                .Include(t => t.Manager)
                .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentPublisher is null)
                return BadRequest();
            var ExistentVideoGame = await _context.VideoGames
                .Include(c => c.Genre)
                .FirstOrDefaultAsync(q => q.Id == id && q.PublisherId == CurrentPublisher.PublisherId);
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
            var genres = _mapper.Map<List<GenreDto>>(
                await _context.Genres
                .ToListAsync());
            if (genres == null)
            {
                return NotFound();
            }
            ViewBag.ExistentGenres = new SelectList(genres, "GenreId", "Name", ExistentVideoGame.GenreId);
            return View(output);
        }

        // POST: Manager/VideoGame/Edit/5
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

            var ExistentVideoGameOffer = await _context.Offers
                        .FirstOrDefaultAsync(q => q.Id == id);

            if (ExistentVideoGameOffer is not null && ModifiedVideoGame.ExistentVideoGame.IsFinal == false)
            {
                TempData["MessageVideoGame"] = $"Offer for <strong>{ModifiedVideoGame.ExistentVideoGame.Title}</strong> needs to be removed before video game can be made not final!";
                return RedirectToAction(nameof(Index));
            }

            if (ExistentVideoGameOffer is not null && ExistentVideoGameOffer.NewPrice >= ModifiedVideoGame.ExistentVideoGame.Price)
            {
                TempData["MessageVideoGame"] = $"Video game <strong>{ModifiedVideoGame.ExistentVideoGame.Title}</strong> price cannot be equal or lower than the price of its offer!";
                return RedirectToAction(nameof(Index));
            }

            var history = await _context.VideoGames
                .Where(i => i.Title == ModifiedVideoGame.ExistentVideoGame.Title && i.Id!=id)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageVideoGame"] = $"The video game <strong>{ModifiedVideoGame.ExistentVideoGame.Title}</strong> already exists!";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var CurrentPublisher = await _context.PublisherManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
                    if (CurrentPublisher is null)
                        return BadRequest();
                    var ExistentVideoGame = await _context.VideoGames
                        .FirstOrDefaultAsync(q => q.Id == id && q.PublisherId == CurrentPublisher.PublisherId);
                    if (ExistentVideoGame == null)
                    {
                        return NotFound();
                    }
                    ModifiedVideoGame.ExistentVideoGame.ToEntity(ref ExistentVideoGame);
                    _context.Update(ExistentVideoGame);
                    await _context.SaveChangesAsync();

                    if (ExistentVideoGame!.IsFinal == false || ExistentVideoGame!.Stock == 0)
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
            var genres = _mapper.Map<List<GenreDto>>(
                await _context.Genres
                .ToListAsync());
            if (genres == null)
            {
                return NotFound();
            }
            ViewBag.ExistentGenres = new SelectList(genres, "GenreId", "Name", ModifiedVideoGame.ExistentVideoGame.GenreId);
            return View(ModifiedVideoGame);
        }

        // GET: Manager/VideoGame/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.VideoGames == null)
            {
                return NotFound();
            }

            var videoGame = await _context.VideoGames
                .Include(c => c.Publisher)
                .Include(t => t.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (videoGame == null)
            {
                return NotFound();
            }

            if (videoGame != null && videoGame.IsFinal is true)
            {
                TempData["MessageVideoGame"] = $"The video game <strong>{videoGame.Title}</strong> cannot be deleted if it is in the final stage!";
                return RedirectToAction(nameof(Index));
            }

            var ExistentVideoGameOffer = await _context.Offers
                        .FirstOrDefaultAsync(q => q.Id == id);

            if (ExistentVideoGameOffer is not null)
            {
                TempData["MessageVideoGame"] = $"The offer for <strong>{videoGame.Title}</strong> needs to be removed before the video game can be deleted!";
                return RedirectToAction(nameof(Index));
            }

            var ListDevelopers = await _context.DevelopersVideoGame
                .Include(t => t.Developer).Where(c => c.VideoGameId == id)
                .OrderBy(o => o.Developer!.LastName)
                .ThenBy(o => o.Developer!.FirstName)
                .ToListAsync();
            var output = _mapper.Map<VideoGameDetailsVm>(videoGame);
            var developers = "";
            foreach (var developer in ListDevelopers)
            {
                developers += $"{developer.Developer!.LastName} {developer.Developer!.FirstName}, ";
            }
            output.Developers = string.IsNullOrEmpty(developers) ? "-" : developers.Substring(0, developers.Length - 2);
            return View(output);
        }

        // POST: Manager/VideoGame/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.VideoGames == null)
            {
                return Problem("Entity set 'ApplicationDbContext.VideoGames' is null.");
            }

            var ExistentVideoGameOffer = await _context.Offers
                        .FirstOrDefaultAsync(q => q.Id == id);

            if (ExistentVideoGameOffer is not null)
            {
                TempData["MessageVideoGame"] = $"Its offer needs to be removed before the video game can be deleted!";
                return RedirectToAction(nameof(Index));
            }

            var videoGame = await _context.VideoGames.FindAsync(id);
            if (videoGame != null)
            {
                if(videoGame.IsFinal is false)
                    _context.VideoGames.Remove(videoGame);
                else TempData["MessageVideoGame"] = $"The video game <strong>{videoGame.Title}</strong> cannot be deleted if it is in the final stage!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Manager/VideoGame/Offer/5
        public async Task<IActionResult> Offer(int? id)
        {
            if (id == null || _context.VideoGames == null)
            {
                return NotFound();
            }

            var videoGame = await _context.VideoGames
                .Include(c => c.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal);
            if (videoGame == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var ListDevelopers = await _context.DevelopersVideoGame
                .Include(t => t.Developer).Where(c => c.VideoGameId == id)
                .OrderBy(o => o.Developer!.LastName)
                .ThenBy(o => o.Developer!.FirstName)
                .ToListAsync();

            var output = _mapper.Map<VideoGameOfferDto>(videoGame);
            var developers = "";
            foreach (var developer in ListDevelopers)
            {
                developers += $"{developer.Developer!.FirstName} {developer.Developer!.LastName}, ";
            }
            output.Developers = string.IsNullOrEmpty(developers) ? "-" : developers.Substring(0, developers.Length - 2);
            var Offer = await _context.Offers.FindAsync(id);
            if (Offer is not null)
            {
                output.PromoText = Offer.PromoText;
                output.NewPrice = Offer.NewPrice;
                output.IsNewOffer = false;
            }
            return View(output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Offer([FromRoute] int? id, [FromForm] VideoGameOfferDto NewOffer)
        {
            if (id == null || _context.VideoGames == null)
            {
                return NotFound();
            }
            var CurrentPublisher = await _context.PublisherManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentPublisher is null)
                return BadRequest();
            var ExistentVideoGame = await _context.VideoGames
                .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal && m.PublisherId == CurrentPublisher.PublisherId);
            if (ExistentVideoGame == null)
            {
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                var ExistentOffer = await _context.Offers
                    .Include(t => t.VideoGame)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (ExistentOffer is null)
                {
                    // add offer
                    var Offer = _mapper.Map<VideoGameOffer>(NewOffer);
                    await _context.AddAsync(Offer);
                }
                else
                {
                    // modify offer
                    ExistentOffer.NewPrice = NewOffer.NewPrice;
                    ExistentOffer.PromoText = NewOffer.PromoText;
                    _context.Update(ExistentOffer);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(NewOffer);
        }

        private bool VideoGameExists(int id)
        {
            return (_context.VideoGames?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

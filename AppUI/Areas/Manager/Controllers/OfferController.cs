using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Areas.Manager.Models.ViewModels;
using AppUI.Data;
using AppUI.Models.Entities;

namespace AppUI.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class OfferController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OfferController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Manager/Offer
        [Route("Manager/Offers")]
        public async Task<IActionResult> Index()
        {
            var CurrentPublisher = await _context.PublisherManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentPublisher == null)
                return Problem("You haven't been assigned to a publisher yet!");
            var offers = _context.Offers
                .Include(t => t.VideoGame)
                .Where(c => c.VideoGame!.PublisherId == CurrentPublisher.PublisherId);
            return View(_mapper.Map<List<VideoGameOfferVm>>(await offers.ToListAsync()));
        }
        

        // GET: Manager/Offer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }
            var CurrentPublisher = await _context.PublisherManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentPublisher is null)
                return BadRequest();

            var ExistentOffer = await _context.Offers
                .Include(t => t.VideoGame)
                .FirstOrDefaultAsync(q => q.Id == id && q.VideoGame!.PublisherId == CurrentPublisher.PublisherId);
            if (ExistentOffer == null)
            {
                return NotFound();
            }
            var output = _mapper.Map<VideoGameOfferDto>(ExistentOffer);

            var ListDevelopers = await _context.DevelopersVideoGame
                .Include(t => t.Developer).Where(c => c.VideoGameId == id)
                .OrderBy(o => o.Developer!.LastName)
                .ThenBy(o => o.Developer!.FirstName)
                .ToListAsync();
            var developers = "";
            foreach (var developer in ListDevelopers)
            {
                developers += $"{developer.Developer!.FirstName} {developer.Developer!.LastName}, ";
            }
            output.Developers = string.IsNullOrEmpty(developers) ? "-" : developers.Substring(0, developers.Length - 2);

            return View(output);
        }

        // GET: Manager/Offer/Create
        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.VideoGames, "Id", "Id");
            return View();
        }

        // POST: Manager/Offer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PromoText,NewPrice")] VideoGameOffer videoGameOffer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(videoGameOffer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.VideoGames, "Id", "Id", videoGameOffer.Id);
            return View(videoGameOffer);
        }

        // GET: Manager/Offer/Edit/5
        [Route("Manager/Offer/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var videoGameOffer = await _context.Offers.FindAsync(id);
            if (videoGameOffer == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.VideoGames, "Id", "Id", videoGameOffer.Id);
            return View(videoGameOffer);
        }

        // POST: Manager/Offer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PromoText,NewPrice")] VideoGameOffer videoGameOffer)
        {
            if (id != videoGameOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(videoGameOffer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoGameOfferExists(videoGameOffer.Id))
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
            ViewData["Id"] = new SelectList(_context.VideoGames, "Id", "Id", videoGameOffer.Id);
            return View(videoGameOffer);
        }

        // GET: Manager/Offer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var CurrentPublisher = await _context.PublisherManagers
                        .Include(t => t.Manager)
                        .Where(q => q.Manager!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentPublisher is null)
                return BadRequest();

            var videoGameOffer = await _context.Offers
                .Include(c => c.VideoGame)
                .FirstOrDefaultAsync(m => m.Id == id && m.VideoGame!.PublisherId == CurrentPublisher.PublisherId);
            if (videoGameOffer == null)
            {
                return NotFound();
            }

            return View(videoGameOffer);
        }

        // POST: Manager/Offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Offers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Offers' is null.");
            }
            var videoGameOffer = await _context.Offers.FindAsync(id);
            if (videoGameOffer != null)
            {
                _context.Offers.Remove(videoGameOffer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VideoGameOfferExists(int id)
        {
            return (_context.Offers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<ActionResult> GetOffer([FromRoute] int id)
        {
            var videoGame = await _context.VideoGames
                .Include(c => c.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsFinal);
            if (videoGame == null)
            {
                return Json(new { success = false, errors = new string[] { "Video game does not exist!" } });
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
                developers += $"{developer.Developer!.LastName} {developer.Developer!.FirstName}, ";
            }
            output.Developers = string.IsNullOrEmpty(developers) ? "-" : developers.Substring(0, developers.Length - 2);
            var Offer = await _context.Offers.FindAsync(id);
            if (Offer is not null)
            {
                output.PromoText = Offer.PromoText;
                output.NewPrice = Offer.NewPrice;
                output.IsNewOffer = false;
            }
            if (Offer is null)
            {
                //return Json(new { success = false, errors = new string[] { "Offer does not exist!" } });
            }
            return Json(new { success = true, item = output });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOffer([FromRoute] int id, [FromForm] AddEditVideoGameOfferDto IntroducedOffer)
        {
            var ExistentOffer = await _context.Offers.FindAsync(id);
            var ExistentVideoGame = await _context.VideoGames
                .FirstOrDefaultAsync(i => i.Id == id && i.IsFinal);
            if (ExistentVideoGame is null)
            {
                return Json(new { success = false, errors = new string[] { "Video game does not exist!" } });
            }

            if (IntroducedOffer.Id != ExistentVideoGame.Id)
            {
                return Json(new { success = false, errors = new string[] { "Bad Request!" } });
            }
            if (ModelState.IsValid)
            {
                if (IntroducedOffer.NewPrice >= ExistentVideoGame.Price)
                {
                    return Json(new { success = false, priceError = true, item=_mapper.Map<VideoGamePriceVm>(ExistentVideoGame), errors = new string[] { "New price needs to be smaller than the old price!" } });
                }
                if (ExistentOffer is null)
                {
                    // insert new offer
                    var NewOffer = _mapper.Map<VideoGameOffer>(IntroducedOffer);
                    Console.WriteLine($"New offer: {IntroducedOffer}, {NewOffer}");

                    //NewOffer.Id = id;
                    await _context.AddAsync(NewOffer);
                }
                else
                {
                    // modify existent offer
                    ExistentOffer.PromoText = IntroducedOffer.PromoText;
                    ExistentOffer.NewPrice = IntroducedOffer.NewPrice;
                    _context.Update(ExistentOffer);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, item = IntroducedOffer });
            }
            var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage));
            return Json(new { success = false, errors });
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteOffer([FromRoute] int id, int VideoGameId)
        {
            var ExistentOffer = await _context.Offers.FindAsync(id);
            if (ExistentOffer is null)
            {
                return Json(new { success = false, errors = new string[] { "Offer does not exist" } });
            }
            if (id != VideoGameId)
            {
                return Json(new { success = false, errors = new string[] { "Bad Request!" } });
            }
            _context.Offers.Remove(ExistentOffer);
            await _context.SaveChangesAsync();
            return Json(new { success = true, item = ExistentOffer });
        }
    }
}

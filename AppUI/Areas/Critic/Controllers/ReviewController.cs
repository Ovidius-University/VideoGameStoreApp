using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppUI.Data;
using AppUI.Models.Entities;
using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Identity;
using AppUI.Areas.Critic.Models.ViewModels;
using AppUI.Areas.Critic.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AppUI.Areas.Critic.Controllers
{
    [Area("Critic")]
    [Authorize(Roles = "Critic")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public ReviewController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: Critic/Review
        [Route("Critic/Reviews")]
        public async Task<IActionResult> Index()
        {
            var CurrentReviewer = await _context.ReviewerCritics
                .Include(t => t.Reviewer)
                .Include(t => t.Critic)
                .FirstOrDefaultAsync(q => q.Critic!.UserName == User.Identity!.Name);
            if (CurrentReviewer == null)
                return Problem("You haven't been assigned to a reviewer yet!");
            var reviews = _mapper.Map<List<ReviewVm>>(
                await _context.Reviews
                .Include(c => c.VideoGame)
                .Where(q => q.ReviewerId == CurrentReviewer.ReviewerId).ToListAsync());
            var output = new IndexReviewsVm()
            {
                Reviewer = CurrentReviewer.Reviewer!.Name,
                ListReviews = reviews
            };
            return View(output);
        }

        // GET: Critic/Review
        [Route("Critic/Review/Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(c => c.Reviewer)
                .Include(c => c.VideoGame)
                .FirstOrDefaultAsync(m => m.VideoGameId == id);
            if (review == null)
            {
                return NotFound();
            }
            var ListReviewers = await _context.Reviews
                .Include(t => t.Reviewer)
                .Where(c => c.VideoGameId == id)
                .OrderBy(o => o.Reviewer!.Name)
                .ToListAsync();

            var output = _mapper.Map<ReviewDetailsVm>(review);
            var reviewers = "";
            foreach (var reviewer in ListReviewers)
            {
                reviewers += $"{reviewer.Reviewer!.Name}";
            }
            output.Reviewer = string.IsNullOrEmpty(reviewers) ? "-" : reviewers.Substring(0, reviewers.Length);
            return View(output);
        }

        // GET: Critic/Review/Create
        public async Task<IActionResult> Create()
        {
            if (_context.Reviewers == null || _context.VideoGames == null)
            {
                return NotFound();
            }

            var CurrentReviewer = await _context.ReviewerCritics
                .Include(t => t.Reviewer)
                .Include(t => t.Critic)
                .FirstOrDefaultAsync(q => q.Critic!.UserName == User.Identity!.Name);

            if (CurrentReviewer == null)
                return NotFound();//redirectionez
            var videoGames = _mapper.Map<List<VideoGameDto>>(
                await _context.VideoGames
                .Where(q => q.IsFinal).ToListAsync());
            ViewBag.UnreviewedVideoGames = new SelectList(videoGames, "VideoGameId", "Title");
            return View();
        }

        // POST: Critic/Review/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] NewReviewDto newReview)
        {
                var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
                var CurrentReviewer = await _context.ReviewerCritics.Where(q => q.CriticId == CurrentUser!.Id).FirstOrDefaultAsync();
                if (CurrentReviewer == null)
                    return BadRequest();
                if (ModelState.IsValid)
                {
                    var history = await _context.Reviews
                    .Where(q => q.ReviewerId == CurrentReviewer!.ReviewerId && q.VideoGameId == newReview!.VideoGameId)
                    .FirstOrDefaultAsync();
                    if (history != null)
                {
                    TempData["MessageReview"] = $"There is already a review from the company for that video game!";
                    return RedirectToAction(nameof(Index));
                }

                    var NewReview = _mapper.Map<Review>(newReview);
                    NewReview.ReviewerId = CurrentReviewer!.ReviewerId;
                    _context.Add(NewReview);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(newReview);
        }

        // GET: Critic/Review/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }
            var CurrentReviewer = await _context.ReviewerCritics
                .Include(t => t.Critic)
                .Where(q => q.Critic!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (CurrentReviewer is null)
                return BadRequest();
            var ExistentReview = await _context.Reviews
                .Include(t => t.VideoGame)
                .Include(t => t.Reviewer)
                .FirstOrDefaultAsync(q => q.VideoGameId == id && q.ReviewerId == CurrentReviewer.ReviewerId);
            if (ExistentReview == null)
            {
                return NotFound();
            }
            var output = _mapper.Map<ExistentReviewDto>(ExistentReview);
            return View(output);
        }

        // POST: Critic/Review/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ExistentReviewDto ModifiedReview)
        {
            if (ModifiedReview is null || id != ModifiedReview.VideoGameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var CurrentReviewer = await _context.ReviewerCritics
                        .Include(t => t.Critic)
                        .Where(q => q.Critic!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
                    if (CurrentReviewer is null)
                        return BadRequest();
                    var ExistentReview = await _context.Reviews
                        .FirstOrDefaultAsync(q => q.VideoGameId == id && q.ReviewerId == CurrentReviewer.ReviewerId);
                    if (ExistentReview == null)
                    {
                        return NotFound();
                    }
                    ModifiedReview.ToEntity(ref ExistentReview);
                    _context.Update(ExistentReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(ModifiedReview.ReviewerId, ModifiedReview.VideoGameId))
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
            var reviewer = await _context.ReviewerCritics
                        .Include(t => t.Critic)
                        .Where(q => q.Critic!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (reviewer is null)
                return BadRequest();
            return View(ModifiedReview);
        }

        // GET: Critic/Review/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }
            var company = await _context.ReviewerCritics
                        .Include(t => t.Critic)
                        .Where(q => q.Critic!.UserName == User!.Identity!.Name).FirstOrDefaultAsync();
            if (company is null)
                return BadRequest();

            var review = await _context.Reviews
                .Include(c => c.Reviewer)
                .Include(c => c.VideoGame)
                .FirstOrDefaultAsync(m => m.VideoGameId == id && m.ReviewerId == company.ReviewerId);
            if (review == null)
            {
                return NotFound();
            }

            if (review != null && review.IsFinal is true)
            {
                TempData["MessageReview"] = $"The review cannot be deleted if it is in the final stage!";
                return RedirectToAction(nameof(Index));
            }

            var Reviewers = await _context.Reviews
                .Include(t => t.Reviewer)
                .Where(c => c.VideoGameId == id && c.ReviewerId == company.ReviewerId)
                .OrderBy(o => o.Reviewer!.Name)
                .ToListAsync();
            var output = _mapper.Map<ReviewDetailsVm>(review);
            var reviewers = "";
            foreach (var reviewer in Reviewers)
            {
                reviewers += $"{reviewer.Reviewer!.Name}";
            }
            output.Reviewer = string.IsNullOrEmpty(reviewers) ? "-" : reviewers.Substring(0, reviewers.Length);
            return View(output);
        }

        // POST: Critic/Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
            var CurrentReviewer = await _context.ReviewerCritics.Where(q => q.CriticId == CurrentUser!.Id).FirstOrDefaultAsync();
            if (CurrentReviewer == null)
                return BadRequest();

            if (_context.Reviews == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reviews' is null.");
            }

            var review = await _context.Reviews.FindAsync(id, CurrentReviewer.ReviewerId);
            if (review != null)
            {
                if (review.IsFinal is false)
                    _context.Reviews.Remove(review);
                else TempData["MessageReview"] = $"The review cannot be deleted if it is in the final stage!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int rid, int vid)
        {
          return (_context.Reviews?.Any(e => e.ReviewerId == rid && e.VideoGameId == vid)).GetValueOrDefault();
        }
    }
}

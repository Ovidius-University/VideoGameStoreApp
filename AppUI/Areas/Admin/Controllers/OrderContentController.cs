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
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Areas.Admin.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using AppUI.Areas.Manager.Models.DTOs;
//using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace AppUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]

public class OrderContentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public OrderContentController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Admin/OrderContents/{oid}")]
    public async Task<IActionResult> Index(int oid)
    {
        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.VideoGame)
            .Include(c => c.VideoGame!.Offer)
            .Where(q => q.OrderId == oid).ToListAsync());
        var output = new IndexOrderContentsVm()
        {
            OrderId = oid,
            ListOrderContents = ordercontents
        };
        return View(output);
    }

    // GET: OrderContent/Details/5
    [Route("Admin/OrderContent/Details/{oid}/{id}")]
    public async Task<IActionResult> Details(int? oid, int? id)
    {
        if (id == null || _context.OrderContents == null)
        {
            return NotFound();
        }

        var ordercontent = await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.VideoGame)
            .Include(c => c.VideoGame!.Offer)
            .FirstOrDefaultAsync(m => m.OrderId == oid && m.VideoGameId == id);
        if (ordercontent == null|| ordercontent.Order == null || ordercontent.Order.Customer == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<OrderContentDetailsVm>(ordercontent);
        output.Customer = string.IsNullOrEmpty(ordercontent.Order.Customer.UserName) ? "-" : ordercontent.Order.Customer.UserName;
        return View(output);
    }

    // GET: OrderContent/Create
    public async Task<IActionResult> Create(int oid)
    {
        if (_context.Users == null || _context.VideoGames == null)
        {
            return NotFound();
        }

        var videoGames = _mapper.Map<List<Models.DTOs.VideoGameDto>>(
            await _context.VideoGames
            .Where(q => q.IsFinal && q.Stock>0).ToListAsync());
        ViewBag.UnaddedVideoGames = new SelectList(videoGames, "VideoGameId", "Title");
        
        var vid = await _context.VideoGames
            .Where(q => q.IsFinal && q.Stock > 0)
            .OrderBy(q => q.Id)
            .FirstOrDefaultAsync();
        if(vid == null)
        {
            TempData["MessageOrderContent"] = $"No video game can be added to the order right now!";
            return RedirectToAction(nameof(Index), new { oid });
        }
        var newOrderContent = new NewOrderContentDto()
        {
            OrderId = oid,
            Quantity = 1,
            VideoGameId = vid.Id
        };
        
        return View(newOrderContent);
    }

    // POST: OrderContent/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromQuery]int oid, [FromForm] NewOrderContentDto newOrderContent)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return BadRequest();

        var videoGame = await _context.VideoGames
                              .Where(q => q.Id == newOrderContent.VideoGameId)
                              .FirstOrDefaultAsync();
        if (videoGame == null)
            return BadRequest();

        if (ModelState.IsValid)
        {
            var resource = await _context.VideoGames
            .Where(q => q.Id == newOrderContent!.VideoGameId && q.IsFinal)
            .FirstOrDefaultAsync();
            if (resource == null)
            {
                TempData["MessageOrderContent"] = $"The video game is not available anymore!";
                return RedirectToAction(nameof(Index), new { oid=newOrderContent.OrderId });
            }

            var history = await _context.OrderContents
            .Where(q => q.VideoGameId == newOrderContent!.VideoGameId && q.OrderId == newOrderContent.OrderId)
            .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessageOrderContent"] = $"The video game is already in the order!";
                return RedirectToAction(nameof(Index), new { oid=newOrderContent.OrderId });
            }

            if (resource.Stock < newOrderContent.Quantity)
            {
                TempData["MessageOrderContent"] = $"The addition of {newOrderContent.Quantity} is bigger than what is in stock right now ({resource.Stock})!";
                return RedirectToAction(nameof(Index), new { oid = newOrderContent.OrderId });
            }

            var NewOrderContent = _mapper.Map<OrderContent>(newOrderContent);
            NewOrderContent.OrderId = newOrderContent.OrderId;
            _context.Add(NewOrderContent);
            await _context.SaveChangesAsync();

            var reducedVideoGame = await _context.VideoGames
                               .Where(i => i.Id == newOrderContent.VideoGameId)
                               .FirstOrDefaultAsync();
            reducedVideoGame!.Stock -= newOrderContent.Quantity;
            _context.Update(reducedVideoGame);
            await _context.SaveChangesAsync();

            var soldoutShopCart = await _context.ShopCarts
                                      .Where(i => i.VideoGame!.Stock == 0)
                                      .ToListAsync();
            foreach (var item in soldoutShopCart)
            {
                _context.ShopCarts.Remove(item);
                await _context.SaveChangesAsync();
            }

            var reducedShopCarts = await _context.ShopCarts
                .Include(c => c.VideoGame)
                .Include(c => c.VideoGame!.Offer)
                .Where(q => q.VideoGameId == newOrderContent.VideoGameId && q.VideoGame!.Stock < q.Quantity)
                .ToListAsync();
            foreach (var item in reducedShopCarts)
            {
                item.Quantity = item.VideoGame!.Stock;
                _context.ShopCarts.Update(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { oid = newOrderContent.OrderId });
        }
        var videoGames = _mapper.Map<List<Models.DTOs.VideoGameDto>>(
            await _context.VideoGames
            .Where(q => q.IsFinal && q.Stock > 0).ToListAsync());
        ViewBag.UnaddedVideoGames = new SelectList(videoGames, "VideoGameId", "Title");
        var vid = await _context.VideoGames
            .Where(q => q.IsFinal && q.Stock > 0)
            .OrderBy(q => q.Id)
            .FirstOrDefaultAsync();
        if (vid == null)
        {
            TempData["MessageOrderContent"] = $"No video game can be added to the order right now!";
            return RedirectToAction(nameof(Index), new { oid = newOrderContent.OrderId });
        }
        return View(newOrderContent);
    }

    // GET: OrderContent/Edit/5
    public async Task<IActionResult> Edit(int? oid, int? id)
    {
        if (id == null || _context.OrderContents == null || _context.Users == null)
        {
            return NotFound();
        }
        var ExistentOrderContent = await _context.OrderContents
            .Include(t => t.VideoGame)
            .Include(t => t.Order)
            .FirstOrDefaultAsync(q => q.OrderId == oid && q.VideoGameId == id);
        if (ExistentOrderContent == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderContentDto>(ExistentOrderContent);
        return View(output);
    }

    // POST: OrderContent/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] int oid, [FromRoute] int id, [FromForm] ExistentOrderContentDto ModifiedOrderContent)
    {
        if (ModifiedOrderContent is null || id != ModifiedOrderContent.VideoGameId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var resource = await _context.VideoGames
                    .Where(q => q.Id == id && q.IsFinal)
                    .FirstOrDefaultAsync();

                if (resource == null)
                {
                    TempData["MessageOrderContent"] = $"The video game is not available anymore!";
                    return RedirectToAction(nameof(Index), new { oid = ModifiedOrderContent.OrderId });
                }

                var ExistentOrderContent = await _context.OrderContents
                    .FirstOrDefaultAsync(q => q.VideoGameId == id && q.OrderId == ModifiedOrderContent.OrderId);
                if (ExistentOrderContent == null)
                {
                    return NotFound();
                }

                if (resource.Stock < ModifiedOrderContent.Quantity - ExistentOrderContent.Quantity)
                {
                    TempData["MessageOrderContent"] = $"The change to the order ({ModifiedOrderContent.Quantity - ExistentOrderContent.Quantity}) is bigger than what we have in stock right now ({resource.Stock})!";
                    return RedirectToAction(nameof(Index), new { oid = ModifiedOrderContent.OrderId });
                }

                var reducedVideoGame = await _context.VideoGames
                               .Where(i => i.Id == ModifiedOrderContent.VideoGameId)
                               .FirstOrDefaultAsync();
                reducedVideoGame!.Stock -= ModifiedOrderContent.Quantity - ExistentOrderContent.Quantity;
                _context.Update(reducedVideoGame);
                await _context.SaveChangesAsync();

                var soldoutShopCart = await _context.ShopCarts
                                          .Where(i => i.VideoGame!.Stock == 0)
                                          .ToListAsync();
                foreach (var item in soldoutShopCart)
                {
                    _context.ShopCarts.Remove(item);
                    await _context.SaveChangesAsync();
                }

                var reducedShopCarts = await _context.ShopCarts
                    .Include(c => c.VideoGame)
                    .Include(c => c.VideoGame!.Offer)
                    .Where(q => q.VideoGameId == ModifiedOrderContent.VideoGameId && q.VideoGame!.Stock < q.Quantity)
                    .ToListAsync();
                foreach (var item in reducedShopCarts)
                {
                    item.Quantity = item.VideoGame!.Stock;
                    _context.ShopCarts.Update(item);
                    await _context.SaveChangesAsync();
                }

                ModifiedOrderContent.ToEntity(ref ExistentOrderContent);
                _context.Update(ExistentOrderContent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderContentExists(ModifiedOrderContent.OrderId, ModifiedOrderContent.VideoGameId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index), new { oid = ModifiedOrderContent.OrderId });
        }

        var ExistenOrderContent = await _context.OrderContents
            .Include(t => t.VideoGame)
            .Include(t => t.Order)
            .FirstOrDefaultAsync(q => q.VideoGameId == id && q.OrderId == ModifiedOrderContent.OrderId);
        if (ExistenOrderContent == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderContentDto>(ExistenOrderContent);
        return View(output);
    }

    // GET: OrderContent/Delete/5
    public async Task<IActionResult> Delete(int? oid, int? id)
    {
        if (id == null || _context.OrderContents == null)
        {
            return NotFound();
        }
        var ordercontent = await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.VideoGame)
            .Include(c => c.VideoGame!.Offer)
            .FirstOrDefaultAsync(m => m.OrderId == oid && m.VideoGameId == id);
        if (ordercontent == null || ordercontent.Order == null || ordercontent.Order.Customer == null)
        {
            return NotFound();
        }

        var Customers = await _context.OrderContents
            .Include(t => t.Order)
            .Where(c => c.VideoGameId == id && c.OrderId == oid)
            .ToListAsync();
        var output = _mapper.Map<OrderContentDetailsVm>(ordercontent);
        output.Customer = string.IsNullOrEmpty(ordercontent.Order.Customer.UserName) ? "-" : ordercontent.Order.Customer.UserName;
        return View(output);
    }

    // POST: OrderContent/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int oid, int id)
    {
        var CurrentUser = await _userManager.FindByEmailAsync(User.Identity!.Name!);
        if (CurrentUser == null)
            return BadRequest();

        if (_context.OrderContents == null)
        {
            return Problem("Entity set 'ApplicationDbContext.OrderContents' is null.");
        }

        var ordercontent = await _context.OrderContents.FindAsync(oid, id);
        if (ordercontent != null)
        {
            _context.OrderContents.Remove(ordercontent);
            await _context.SaveChangesAsync();

            var reducedVideoGame = await _context.VideoGames
                               .Where(i => i.Id == ordercontent.VideoGameId)
                               .FirstOrDefaultAsync();
            reducedVideoGame!.Stock += ordercontent.Quantity;
            _context.Update(reducedVideoGame);
            await _context.SaveChangesAsync();

            var soldoutShopCart = await _context.ShopCarts
                                      .Where(i => i.VideoGame!.Stock == 0)
                                      .ToListAsync();
            foreach (var item in soldoutShopCart)
            {
                _context.ShopCarts.Remove(item);
                await _context.SaveChangesAsync();
            }

            var reducedShopCarts = await _context.ShopCarts
                .Include(c => c.VideoGame)
                .Include(c => c.VideoGame!.Offer)
                .Where(q => q.VideoGameId == ordercontent.VideoGameId && q.VideoGame!.Stock < q.Quantity)
                .ToListAsync();
            foreach (var item in reducedShopCarts)
            {
                item.Quantity = item.VideoGame!.Stock;
                _context.ShopCarts.Update(item);
                await _context.SaveChangesAsync();
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { oid = ordercontent!.OrderId });
    }

    private bool OrderContentExists(int oid, int vid)
    {
      return (_context.OrderContents?.Any(e => e.OrderId == oid && e.VideoGameId == vid)).GetValueOrDefault();
    }
}

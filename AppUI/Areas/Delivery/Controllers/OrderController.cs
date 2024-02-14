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
using AppUI.Areas.Delivery.Models.ViewModels;
using AppUI.Areas.Delivery.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using AppUI.Areas.Manager.Models.DTOs;
//using Elfie.Serialization;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppUI.Areas.Delivery.Controllers;

[Area("Delivery")]
[Authorize(Roles = "Delivery")]

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public OrderController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Delivery/Orders")]
    public async Task<IActionResult> Index()
    {
        var output = new IndexOrdersVm()
        {
            ListOrders = new List<OrderVm>()
        };

        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .ToListAsync();

        foreach (var item in empSched)
        {
            var weekday = item.Day!.Name;

            var Orders = _mapper.Map<List<OrderVm>>(
                await _context.Orders
                .Include(c => c.Customer)
                .Include(c => c.FeedBack)
                .Include(c => c.PayingMethod)
                .Include(c => c.OrderContents)
                .Include(c => c.DeliveryName)
                .Where(c => c.IsDelivery == true 
                 && c.ArrivalTime.TimeOfDay >= item.StartHour && c.ArrivalTime.TimeOfDay <= item.EndHour
                 && (c.DeliveryId == null || c.DeliveryId == 0 || c.DeliveryId == item.EmployeeId))
                .OrderBy(c => c.IsFinal)
                .ThenByDescending(c => c.DeliveryId)
                .ThenBy(c => c.ArrivalTime)
                .ToListAsync());

            if (Orders != null)
            {
                foreach (var i in Orders)
                {
                    if (i.ArrivalTime.DayOfWeek.ToString().Equals(weekday))
                        output.ListOrders.Add(i);
                }
            }
        }

        return View(output);
    }

    // GET: Order/Details/5
    [Route("Delivery/Order/Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.FeedBack)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.DeliveryName)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null||order.Customer == null)
        {
            return NotFound();
        }

        var output = _mapper.Map<OrderDetailsVm>(order);

        output.Customer = string.IsNullOrEmpty(order.Customer.UserName) ? "-" : order.Customer.UserName;
        return View(output);
    }

    // GET: Order/TakeOrder/5
    public async Task<IActionResult> TakeOrder(int? id)
    {
        if (id == null || _context.Orders == null || _context.Users == null)
        {
            return NotFound();
        }
        var ExistentOrder = await _context.Orders
            .Include(t => t.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.DeliveryName)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistentOrder == null)
        {
            return NotFound();
        }
        if (ExistentOrder.IsFinal)
        {
            TempData["MessageDeliveredOrder"] = $"Order number {id} cannot be freed up, as it was confirmed!";
            return RedirectToAction(nameof(Index));
        }

        var output = _mapper.Map<ExistentDeliveryDto>(ExistentOrder);

        var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        if (ExistentOrder.DeliveryId == null || ExistentOrder.DeliveryId == 0) {
            output.DeliveryId = empSched!.EmployeeId;
        }
        else output.DeliveryId = null;
        /*
        var methods = _mapper.Map<List<PayingMethodDto>>(
            await _context.PayingMethods
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
        */
        return View(output);
    }

    // POST: Order/TakeOrder/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TakeOrder([FromRoute] int id, [FromForm] ExistentDeliveryDto ModifiedOrder)
    {
        if (ModifiedOrder is null || id != ModifiedOrder.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var empSched = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .ToListAsync();

                var psbl = false;

                foreach (var item in empSched)
                {
                    var weekday = item.Day!.Name;

                    var Order = await _context.Orders
                        .Include(c => c.Customer)
                        .Include(c => c.PayingMethod)
                        .Include(c => c.OrderContents)
                        .Include(c => c.DeliveryName)
                        .Where(c => c.Id == id
                         && c.IsDelivery == true
                         && c.ArrivalTime.TimeOfDay >= item.StartHour && c.ArrivalTime.TimeOfDay <= item.EndHour
                         && (c.DeliveryId == null || c.DeliveryId == 0 || c.DeliveryId == item.EmployeeId))
                        .FirstOrDefaultAsync();

                    if (Order != null)
                    {
                        if (Order.ArrivalTime.DayOfWeek.ToString().Equals(weekday))
                                psbl = true;
                    }
                }

                if (!psbl)
                {
                    TempData["MessageDeliveredOrder"] = $"Order number {ModifiedOrder.Id} is either no longer a delivery, no longer within your schedule, or was taken by someone else already!";
                    return RedirectToAction(nameof(Index));
                }

                var ExistentOrder = await _context.Orders
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (ExistentOrder == null)
                {
                    return NotFound();
                }

                ModifiedOrder.ToEntity(ref ExistentOrder);
                _context.Update(ExistentOrder);
                await _context.SaveChangesAsync();
                if (ModifiedOrder.DeliveryId != 0 && ModifiedOrder.DeliveryId != null) {
                    TempData["MessageDeliveredOrder"] = $"Order number {ModifiedOrder.Id} has been picked up by you!";
                }
                else
                {
                    TempData["MessageDeliveredOrder"] = $"Order number {ModifiedOrder.Id} has been freed up by you!";
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(ModifiedOrder.Id))
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

        var ExistenOrder = await _context.Orders
            .Include(t => t.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistenOrder == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentDeliveryDto>(ExistenOrder);
        var empSche = await _context.EmployeeSchedules
                        .Include(i => i.Employee)
                        .Include(i => i.Day)
                        .Where(i => i.Employee!.UserName == User.Identity!.Name)
                        .FirstOrDefaultAsync();
        if (ExistenOrder.DeliveryId == null || ExistenOrder.DeliveryId == 0)
        {
            output.DeliveryId = empSche!.EmployeeId;
        }
        else output.DeliveryId = null;
        TempData["MessageDeliveredOrder"] = $"Please complete the necessary information properly!";
        /*
        var payingmethods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
        if (payingmethods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(payingmethods, "PayingMethodId", "Name");
        */
        return View(output);
    }

    // GET: Delivery/Order/FeedBack/5
    [Route("Delivery/Order/FeedBack/{id}")]
    public async Task<IActionResult> FeedBack(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null || order.Customer == null)
        {
            return NotFound();
        }

        var ListVideoGames = await _context.OrderContents
                .Include(t => t.VideoGame)
                .Where(c => c.OrderId == id)
                .ToListAsync();

        var output = _mapper.Map<OrderFeedBackDto>(order);
        var videoGames = _mapper.Map<List<ExistentOrderContentDto>>(ListVideoGames);

        output.VideoGames = videoGames;
        var FeedBack = await _context.FeedBacks.FindAsync(id);
        if (FeedBack is not null)
        {
            output.Comment = FeedBack.Comment;
            output.IsNewFeedBack = false;
        }
        return View(output);
    }

    // GET: Order/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Orders == null || _context.Users == null)
        {
            return NotFound();
        }
        var ExistentOrder = await _context.Orders
            .Include(t => t.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Include(c => c.DeliveryName)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistentOrder == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderDto>(ExistentOrder);
        output.IsFinal = !ExistentOrder.IsFinal;
        /*
        var methods = _mapper.Map<List<PayingMethodDto>>(
            await _context.PayingMethods
            .ToListAsync());
        if (methods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
        */
        return View(output);
    }

    // POST: Order/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ExistentOrderDto ModifiedOrder)
    {
        if (ModifiedOrder is null || id != ModifiedOrder.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var ExistentOrder = await _context.Orders
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (ExistentOrder == null)
                {
                    return NotFound();
                }
                /*
                var specialday = await _context.SpecialDays
                               .Where(i => i.Day == ModifiedOrder.ArrivalTime.Day && i.Month == ModifiedOrder.ArrivalTime.Month)
                               .FirstOrDefaultAsync();
                if (specialday != null)
                {
                    if (specialday.IsWorkDay == false)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done on {specialday.Day}/{specialday.Month}! Please choose a different date!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                    if (ModifiedOrder.ArrivalTime.TimeOfDay < specialday.StartHour || ModifiedOrder.ArrivalTime.TimeOfDay > specialday.EndHour)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done at {ModifiedOrder.ArrivalTime.TimeOfDay} on {specialday.Day}/{specialday.Month}! Please choose a different time!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                }

                var workhour = await _context.WorkHours
                               .Where(i => i.Name.Equals(ModifiedOrder.ArrivalTime.DayOfWeek.ToString()))
                               .FirstOrDefaultAsync();
                if (workhour != null)
                {
                    if (workhour.IsWorkDay == false)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done on {workhour.Name}! Please choose a different date!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                    if (ModifiedOrder.ArrivalTime.TimeOfDay < workhour.StartHour || ModifiedOrder.ArrivalTime.TimeOfDay > workhour.EndHour)
                    {
                        TempData["MessageOrderEdit"] = $"No work is done at {ModifiedOrder.ArrivalTime.TimeOfDay} on {workhour.Name}! Please choose a different time!";
                        var methods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
                        if (methods == null)
                        {
                            return NotFound();
                        }
                        ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                        return View(ModifiedOrder);
                    }
                }

                if (ModifiedOrder.IsDelivery == true && (ModifiedOrder.Address == null || ModifiedOrder.Address.Length == 0))
                {
                    TempData["MessageOrderEdit"] = $"For a delivery, we need the address!";
                    var methods = _mapper.Map<List<PayingMethodDto>>(
                        await _context.PayingMethods
                        .ToListAsync());
                    if (methods == null)
                    {
                        return NotFound();
                    }
                    ViewBag.AvailablePayingMethods = new SelectList(methods, "PayingMethodId", "Name");
                    return View(ModifiedOrder);
                }
                //
                if (ModifiedOrder.IsFinal == true)
                {
                    var specialday = await _context.SpecialDays
                               .Where(i => i.Day == DateTime.Now.Day && i.Month == DateTime.Now.Month)
                               .FirstOrDefaultAsync();
                    if (specialday != null)
                    {
                        if (specialday.IsWorkDay == false)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done on {specialday.Day}/{specialday.Month}! You'll have to confirm the pick-up at a later date!";
                            return View(ModifiedOrder);
                        }
                        if (DateTime.Now.TimeOfDay < specialday.StartHour || DateTime.Now.TimeOfDay > specialday.EndHour)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done at {DateTime.Now.TimeOfDay} on {specialday.Day}/{specialday.Month}! You'll have to confirm the pick-up at a later time!";
                            return View(ModifiedOrder);
                        }
                    }
                    var weekDay = DateTime.Now.DayOfWeek.ToString();
                    var workhour = await _context.WorkHours
                                   .Where(i => i.Name.Equals(weekDay))
                                   .FirstOrDefaultAsync();
                    if (workhour != null)
                    {
                        if (workhour.IsWorkDay == false)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done on {workhour.Name}! You'll have to confirm the pick-up at a later date!";
                            return View(ModifiedOrder);
                        }
                        if (DateTime.Now.TimeOfDay < workhour.StartHour || DateTime.Now.TimeOfDay > workhour.EndHour)
                        {
                            TempData["MessageOrderEdit"] = $"No work is done at {DateTime.Now.TimeOfDay} on {workhour.Name}! You'll have to confirm the pick-up at a later time!";
                            return View(ModifiedOrder);
                        }
                    }
                }
                */

                if (ModifiedOrder.IsFinal == true && ExistentOrder.IsFinal == false)
                {
                    ModifiedOrder.DeliveryTime = DateTime.Now;
                    TempData["MessageDeliveredOrder"] = $"Delivery for order number {ModifiedOrder.Id} for {ModifiedOrder.Name} has been confirmed!";
                }
                if (ModifiedOrder.IsFinal == false && ExistentOrder.IsFinal == true)
                {
                    TempData["MessageDeliveredOrder"] = $"Delivery for order number {ModifiedOrder.Id} for {ModifiedOrder.Name} has been unconfirmed!";

                    var ExistentFeedBack = await _context.FeedBacks
                                        .FirstOrDefaultAsync(q => q.Id == id);
                    if (ExistentFeedBack != null)
                    {
                        _context.Remove(ExistentFeedBack);
                        await _context.SaveChangesAsync();
                    }
                }
                ModifiedOrder.ToEntity(ref ExistentOrder);
                _context.Update(ExistentOrder);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(ModifiedOrder.Id))
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

        var ExistenOrder = await _context.Orders
            .Include(t => t.Customer)
            .Include(c => c.DeliveryName)
            .Include(c => c.PayingMethod)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (ExistenOrder == null)
        {
            return NotFound();
        }
        var output = _mapper.Map<ExistentOrderDto>(ExistenOrder);
        output.IsFinal = !ExistenOrder.IsFinal;
        TempData["MessageOrderEdit"] = $"Please complete the necessary information properly!";
        /*
        var payingmethods = _mapper.Map<List<PayingMethodDto>>(
                            await _context.PayingMethods
                            .ToListAsync());
        if (payingmethods == null)
        {
            return NotFound();
        }
        ViewBag.AvailablePayingMethods = new SelectList(payingmethods, "PayingMethodId", "Name");
        */
        return View(output);
    }
    /*
    // GET: Order/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Orders == null)
        {
            return NotFound();
        }
        
        var Order = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (Order == null|| Order.Customer == null)
        {
            return NotFound();
        }

        if (Order.IsFinal == true)
        {
            TempData["MessageDeliveredOrder"] = $"Order number {Order.Id} cannot be removed as it has been completed!";
            return RedirectToAction(nameof(Index));
        }

        var Customers = await _context.Orders
            .Include(t => t.Customer)
            .Where(c => c.Id == id)
            .ToListAsync();
        var output = _mapper.Map<OrderDetailsVm>(Order);
        output.Customer = string.IsNullOrEmpty(Order.Customer.UserName) ? "-" : Order.Customer.UserName;
        return View(output);
    }

    // POST: Order/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Orders == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Orders' is null.");
        }

        var orderContents = await _context.OrderContents
                    .Include(c => c.Order)
                    .Include(c => c.VideoGame)
                    .Include(c => c.VideoGame!.Offer)
                    .Where(q => q.OrderId == id).ToListAsync();

        foreach (var item in orderContents)
        {
            var stockedVideoGame = await _context.VideoGames
                       .Where(i => i.Id == item.VideoGameId)
                       .FirstOrDefaultAsync();
            stockedVideoGame!.Stock += item.Quantity;
            _context.Update(stockedVideoGame);
            await _context.SaveChangesAsync();
        }

        var Order = await _context.Orders.FindAsync(id);
        if (Order != null)
        {
            _context.Orders.Remove(Order);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    */
    private bool OrderExists(int id)
    {
      return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}

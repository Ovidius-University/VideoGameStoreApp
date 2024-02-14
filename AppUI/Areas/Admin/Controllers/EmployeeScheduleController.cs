using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppUI.Data;
using AppUI.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AppUI.Areas.Manager.Models.DTOs;
using NuGet.Packaging.Signing;

namespace AppUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class EmployeeScheduleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public readonly SignInManager<AppUser> _signInManager;
    public readonly UserManager<AppUser> _userManager;
    public readonly RoleManager<AppRole> _roleManager;

    public EmployeeScheduleController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    [Route("Admin/EmployeeSchedules")]
    // GET: Admin/EmployeeSchedule
    public async Task<IActionResult> Index()
    {
        var employeeschedules = _mapper.Map<List<EmployeeScheduleVm>>(
            await _context.EmployeeSchedules
            .Include(c => c.Employee)
            .Include(c => c.Day)
            .OrderBy(c => c.Day)
            .ThenBy(c => c.Employee)
            .ToListAsync());
        var output = new IndexEmployeeSchedulesVm()
        {
            ListEmployeeSchedules = employeeschedules
        };
        return View(output);
    }

    // GET: Admin/EmployeeSchedule/Details/5
    [Route("Admin/EmployeeSchedule/Details/{eid}/{did}")]
    public async Task<IActionResult> Details(int? eid, int? did)
    {
        if (eid == null || did == null || _context.EmployeeSchedules == null)
        {
            return NotFound();
        }

        var employeeSchedule = _mapper.Map<EmployeeScheduleVm>(
            await _context.EmployeeSchedules
            .Include(i => i.Employee)
            .Include(i => i.Day)
            .FirstOrDefaultAsync(m => m.EmployeeId == eid && m.DayId == did));
        if (employeeSchedule == null)
        {
            return NotFound();
        }

        return View(employeeSchedule);
    }

    // GET: Admin/EmployeeSchedule/Create
    public async Task<IActionResult> Create()
    {
        if (_context.Users == null || _context.WorkHours == null)
        {
            return NotFound();
        }

        var deliverers = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Delivery"));
        var cashiers = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Cashier"));
        if(deliverers == null && cashiers == null)
        {
            TempData["MessageEmployeeSchedule"] = $"No employee (delivery or cashier) has been registered yet!";
            return RedirectToAction(nameof(Index));
        }
        System.Collections.IEnumerable? employees = null;
        if (deliverers == null)
        {
            employees = cashiers;
        }
        else if (cashiers == null)
        {
            employees = deliverers;
        }
        else
        {
            employees = _mapper.Map<List<ExistentUserDto>>(deliverers.Concat(cashiers));
        }
        ViewBag.UnaddedEmployees = new SelectList(employees, "Id", "Email");

        var workHours = _mapper.Map<List<ExistentWorkHourDto>>(
            await _context.WorkHours
            .ToListAsync());
        if (workHours == null)
        {
            TempData["MessageEmployeeSchedule"] = $"No work hour has been registered yet!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
        return View();
    }

    // POST: Admin/EmployeeSchedule/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewEmployeeScheduleDto employeeSchedule)
    {
        if (_context.Users == null || _context.WorkHours == null)
        {
            return NotFound();
        }

        var deliverers = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Delivery"));
        var cashiers = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Cashier"));
        if (deliverers == null && cashiers == null)
        {
            TempData["MessageEmployeeSchedule"] = $"No employee (delivery or cashier) has been registered yet!";
            return RedirectToAction(nameof(Index));
        }
        System.Collections.IEnumerable? employees = null;
        if (deliverers == null)
        {
            employees = cashiers;
        }
        else if (cashiers == null)
        {
            employees = deliverers;
        }
        else
        {
            employees = _mapper.Map<List<ExistentUserDto>>(deliverers.Concat(cashiers));
        }

        var workHours = _mapper.Map<List<ExistentWorkHourDto>>(
            await _context.WorkHours
            .ToListAsync());
        if (workHours == null)
        {
            TempData["MessageEmployeeSchedule"] = $"No work hour has been registered yet!";
            return RedirectToAction(nameof(Index));
        }

        var history = _mapper.Map<List<ExistentEmployeeScheduleDto>>(
            await _context.EmployeeSchedules
            .Include(i => i.Employee)
            .Include(i => i.Day)
            .Where(i => i.EmployeeId == employeeSchedule.EmployeeId && i.DayId == employeeSchedule.DayId)
            .ToListAsync());

        if (history != null && history.Count>0)
        {
            TempData["MessageEmployeeSchedule"] = $"There is already a work day schedule for employee {employeeSchedule.Employee} on that day!";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            var workhour = await _context.WorkHours
                               .Where(i => i.Id.Equals(employeeSchedule.DayId))
                               .FirstOrDefaultAsync();
            if (workhour == null)
            {
                TempData["MessageEmployeeSchedule"] = $"The work day has not been registered yet!";
                return RedirectToAction(nameof(Index));
            }
            if (workhour != null)
            {
                if (workhour.IsWorkDay == false)
                {
                    TempData["MessageEmployeeScheduleAdd"] = $"No work is done on {workhour.Name}! Please choose a different date!";
                    ViewBag.UnaddedEmployees = new SelectList(employees, "Id", "Email");
                    ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
                    return View(employeeSchedule);
                }
                if (employeeSchedule.StartHour < workhour.StartHour || employeeSchedule.StartHour > workhour.EndHour)
                {
                    TempData["MessageEmployeeScheduleAdd"] = $"Start time for employee {employeeSchedule.Employee} needs to be between {workhour.StartHour} and {workhour.EndHour} on {workhour.Name}! Please choose a different time!";
                    ViewBag.UnaddedEmployees = new SelectList(employees, "Id", "Email");
                    ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
                    return View(employeeSchedule);
                }
                if (employeeSchedule.EndHour < workhour.StartHour || employeeSchedule.EndHour > workhour.EndHour)
                {
                    TempData["MessageEmployeeScheduleAdd"] = $"End time for employee {employeeSchedule.Employee} needs to be between {workhour.StartHour} and {workhour.EndHour} on {workhour.Name}! Please choose a different time!";
                    ViewBag.UnaddedEmployees = new SelectList(employees, "Id", "Email");
                    ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
                    return View(employeeSchedule);
                }
            }
            var newEmployeeSchedule = _mapper.Map<EmployeeSchedule>(employeeSchedule);
            _context.Add(newEmployeeSchedule);
            await _context.SaveChangesAsync();
            TempData["MessageEmployeeSchedule"] = $"The employee <strong>{employeeSchedule.Employee}</strong> was successfully scheduled on {workhour!.Name} at hours {employeeSchedule.StartHour} - {employeeSchedule.EndHour}!";
            return RedirectToAction(nameof(Index));
        }
        /*
        ViewData["DayId"] = new SelectList(_context.WorkHours, "Id", "Id", employeeSchedule.DayId);
        ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Id", employeeSchedule.EmployeeId);
        */
        TempData["MessageEmployeeScheduleAdd"] = $"Please complete the necessary information properly!";
        ViewBag.UnaddedEmployees = new SelectList(employees, "Id", "Email");
        ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
        return View(employeeSchedule);
    }

    // GET: Admin/EmployeeSchedule/Edit/5
    public async Task<IActionResult> Edit(int? eid, int? did)
    {
        if (eid == null || did == null || _context.EmployeeSchedules == null)
        {
            return NotFound();
        }

        var employeeSchedule = _mapper.Map<ExistentEmployeeScheduleDto>(
            await _context.EmployeeSchedules
            .Include(i => i.Employee)
            .Include(i => i.Day)
            .FirstOrDefaultAsync(m => m.EmployeeId == eid && m.DayId == did));
        if (employeeSchedule == null)
        {
            return NotFound();
        }

        return View(employeeSchedule);
    }

    // POST: Admin/EmployeeSchedule/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? eid, int? did, [FromForm] ExistentEmployeeScheduleDto modifiedEmployeeSchedule)
    {
        if (eid == null || did == null || _context.EmployeeSchedules == null)
        {
            return NotFound();
        }

        if (eid != modifiedEmployeeSchedule.EmployeeId && did != modifiedEmployeeSchedule.DayId)
        {
            return NotFound();
        }
        var employeeSchedule = await _context.EmployeeSchedules.Include(i => i.Employee).Include(i => i.Day).FirstOrDefaultAsync(m => m.EmployeeId == eid && m.DayId == did);
        if (employeeSchedule == null)
        {
            return NotFound();
        }
        if (employeeSchedule.EmployeeId != modifiedEmployeeSchedule.EmployeeId || employeeSchedule.DayId != modifiedEmployeeSchedule.DayId)
        {
            return BadRequest();
        }

        var deliverers = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Delivery"));
        var cashiers = _mapper.Map<List<ExistentUserDto>>(
            await _userManager.GetUsersInRoleAsync("Cashier"));
        if (deliverers == null && cashiers == null)
        {
            TempData["MessageEmployeeSchedule"] = $"No employee (delivery or cashier) has been registered yet!";
            return RedirectToAction(nameof(Index));
        }
        System.Collections.IEnumerable? employees = null;
        if (deliverers == null)
        {
            employees = cashiers;
        }
        else if (cashiers == null)
        {
            employees = deliverers;
        }
        else
        {
            employees = _mapper.Map<List<ExistentUserDto>>(deliverers.Concat(cashiers));
        }

        var workHours = _mapper.Map<List<ExistentWorkHourDto>>(
            await _context.WorkHours
            .ToListAsync());
        if (workHours == null)
        {
            TempData["MessageEmployeeSchedule"] = $"No work hour has been registered yet!";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            try
            {
                var workhour = await _context.WorkHours
                               .Where(i => i.Id.Equals(modifiedEmployeeSchedule.DayId))
                               .FirstOrDefaultAsync();
                if (workhour == null)
                {
                    TempData["MessageEmployeeSchedule"] = $"The work day has not been registered yet!";
                    return RedirectToAction(nameof(Index));
                }
                if (workhour != null)
                {
                    if (workhour.IsWorkDay == false)
                    {
                        TempData["MessageEmployeeScheduleEdit"] = $"No work is done on {workhour.Name}! Please choose a different date!";
                        ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
                        return View(modifiedEmployeeSchedule);
                    }
                    if (modifiedEmployeeSchedule.StartHour < workhour.StartHour || modifiedEmployeeSchedule.StartHour > workhour.EndHour)
                    {
                        TempData["MessageEmployeeScheduleEdit"] = $"Start time for employee ({modifiedEmployeeSchedule.Employee}) needs to be between {workhour.StartHour} and {workhour.EndHour} on {workhour.Name}! Please choose a different time!";
                        ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
                        return View(modifiedEmployeeSchedule);
                    }
                    if (modifiedEmployeeSchedule.EndHour < workhour.StartHour || modifiedEmployeeSchedule.EndHour > workhour.EndHour)
                    {
                        TempData["MessageEmployeeScheduleEdit"] = $"End time for employee ({modifiedEmployeeSchedule.Employee}) needs to be between {workhour.StartHour} and {workhour.EndHour} on {workhour.Name}! Please choose a different time!";
                        ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
                        return View(modifiedEmployeeSchedule);
                    }
                }

                var weekday = modifiedEmployeeSchedule.Day;

                var orders = await _context.Orders
                               .Where(i => i.DeliveryId.Equals(modifiedEmployeeSchedule.EmployeeId) && i.IsFinal == false
                                && (i.ArrivalTime.TimeOfDay < modifiedEmployeeSchedule.StartHour || i.ArrivalTime.TimeOfDay > modifiedEmployeeSchedule.EndHour))
                               .FirstOrDefaultAsync();

                if (orders != null && orders.ArrivalTime.DayOfWeek.ToString().Equals(weekday))
                {
                    TempData["MessageEmployeeScheduleEdit"] = $"Employee {modifiedEmployeeSchedule.Employee} has an order to deliver at {orders.ArrivalTime.TimeOfDay} on {orders.ArrivalTime.Day}/{orders.ArrivalTime.Month} which would be outside of the new schedule! Please wait until the order is delivered or modify the order itself!";
                    ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
                    return View(modifiedEmployeeSchedule);
                }

                TempData["MessageEmployeeSchedule"] = $"We modified work hours for <strong>{modifiedEmployeeSchedule.Employee}</strong> on <strong>{modifiedEmployeeSchedule.Day}</strong> to {modifiedEmployeeSchedule.StartHour} - {modifiedEmployeeSchedule.EndHour}!";
                modifiedEmployeeSchedule.ToEntity(ref employeeSchedule);
                _context.Update(employeeSchedule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeScheduleExists(employeeSchedule.EmployeeId))
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
        ViewBag.WorkHours = new SelectList(workHours, "Id", "Name");
        return View(modifiedEmployeeSchedule);
    }

    // GET: Admin/EmployeeSchedule/Delete/5
    public async Task<IActionResult> Delete(int? eid, int? did)
    {
        if (eid == null || did == null || _context.EmployeeSchedules == null)
        {
            return NotFound();
        }

        var employeeSchedule = _mapper.Map<EmployeeScheduleVm>(await _context.EmployeeSchedules
            .Include(i => i.Employee)
            .Include(i => i.Day)
            .FirstOrDefaultAsync(m => m.EmployeeId == eid && m.DayId == did));
        if (employeeSchedule == null)
        {
            return NotFound();
        }

        return View(employeeSchedule);
    }

    // POST: Admin/EmployeeSchedule/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? eid, int? did)
    {
        if (eid == null || did == null || _context.EmployeeSchedules == null)
        {
            return Problem("Entity set 'ApplicationDbContext.EmployeeSchedules'  is null.");
        }

        var employeeSchedule = await _context.EmployeeSchedules
            .Include(i => i.Employee)
            .Include(i => i.Day)
            .FirstOrDefaultAsync(m => m.EmployeeId == eid && m.DayId == did);
        if (employeeSchedule == null)
        {
            return NotFound();
        }
        if (employeeSchedule != null)
        {
            var weekday = employeeSchedule.Day!.Name;

            var orders = await _context.Orders
                               .Where(i => i.DeliveryId.Equals(employeeSchedule.EmployeeId) && i.IsFinal == false)
                               .FirstOrDefaultAsync();

            if (orders != null && orders.ArrivalTime.DayOfWeek.ToString().Equals(weekday))
            {
                TempData["MessageEmployeeSchedule"] = $"Employee {employeeSchedule.Employee} has an order to deliver at {orders.ArrivalTime.TimeOfDay} on {orders.ArrivalTime.Day}/{orders.ArrivalTime.Month}! Please wait until the order is delivered or modify the order itself!";
                return RedirectToAction(nameof(Index));
            }

            _context.EmployeeSchedules.Remove(employeeSchedule);
        }
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EmployeeScheduleExists(int id)
    {
      return (_context.EmployeeSchedules?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
    }
}

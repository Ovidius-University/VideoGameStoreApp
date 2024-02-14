﻿using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AppUI.Areas.Admin.Models.DTOs;
using Microsoft.AspNetCore.SignalR;
namespace AppUI.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class UserController : Controller
{
    public readonly IMapper _mapper;
    public readonly ApplicationDbContext _context;
    public readonly SignInManager<AppUser> _signInManager;
    public readonly UserManager<AppUser> _userManager;
    public readonly RoleManager<AppRole> _roleManager;

    public UserController(IMapper mapper, ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    public async Task<IActionResult> Index()
    {
        var users = _mapper.Map<List<ExistentUserDto>>(await _userManager.Users.ToListAsync());
        foreach (var user in users)
        {
            var currentUser = await _userManager.FindByEmailAsync(user.Email);
            user.IsAdmin = await _userManager.IsInRoleAsync(currentUser!, "Administrator");
            user.IsManager = await _userManager.IsInRoleAsync(currentUser!, "Manager");
            user.IsCritic = await _userManager.IsInRoleAsync(currentUser!, "Critic");
            user.IsDelivery = await _userManager.IsInRoleAsync(currentUser!, "Delivery");
            user.IsCashier = await _userManager.IsInRoleAsync(currentUser!, "Cashier");
        }
        return View(users);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var existentUser = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);
        if (existentUser is null)
            return NotFound();
        var output = _mapper.Map<ExistentUserDto>(existentUser);
        output.IsAdmin = await _userManager.IsInRoleAsync(existentUser, "Administrator");
        output.IsManager = await _userManager.IsInRoleAsync(existentUser, "Manager");
        output.IsCritic = await _userManager.IsInRoleAsync(existentUser, "Critic");
        output.IsDelivery = await _userManager.IsInRoleAsync(existentUser, "Delivery");
        output.IsCashier = await _userManager.IsInRoleAsync(existentUser, "Cashier");
        return View(output);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentUserDto modifiedUser)
    {
        var existentUser = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);
        if (existentUser is null)
            return NotFound();
        if (id != modifiedUser.Id || !modifiedUser.Email.Equals(existentUser.Email))
            return BadRequest();
        if (ModelState.IsValid)
        {
            if (!await _roleManager.RoleExistsAsync("Administrator"))
                await _roleManager.CreateAsync(new() { Name = "Administrator" });
            if (!await _roleManager.RoleExistsAsync("Manager"))
                await _roleManager.CreateAsync(new() { Name = "Manager" });
            if (!await _roleManager.RoleExistsAsync("Critic"))
                await _roleManager.CreateAsync(new() { Name = "Critic" });
            if (!await _roleManager.RoleExistsAsync("Delivery"))
                await _roleManager.CreateAsync(new() { Name = "Delivery" });
            if (!await _roleManager.RoleExistsAsync("Cashier"))
                await _roleManager.CreateAsync(new() { Name = "Cashier" });
            StringBuilder message = new();
            var roles = 0;
            if (modifiedUser.IsAdmin) roles++;
            if (modifiedUser.IsManager) roles++;
            if (modifiedUser.IsCritic) roles++;
            if (modifiedUser.IsDelivery) roles++;
            if (modifiedUser.IsCashier) roles++;
            if (roles>1)
            {
                message.Append($"The user <strong>{existentUser.Email}</strong> cannot have more than one role!");
                //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot have more than one role!";
            }
            else
            {
                if (modifiedUser.IsAdmin)
                {
                    if (!await _userManager.IsInRoleAsync(existentUser, "Administrator"))
                    {
                        await _userManager.AddToRoleAsync(existentUser, "Administrator");
                        message.Append($"The user <strong>{existentUser.Email}</strong> is now an <strong>Administrator</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> became an <strong>Administrator</strong>!<br/>";
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still an <strong>Administrator</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still an <strong>Administrator</strong>!";
                    }
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(existentUser, "Administrator"))
                    {
                        await _userManager.RemoveFromRolesAsync(existentUser, new string[] { "Administrator" });
                        message.Append($"The user <strong>{existentUser.Email}</strong> is no longer an <strong>Administrator</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Administrator</strong>!<br/>";
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still not an <strong>Administrator</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still not an <strong>Administrator</strong>!";
                    }
                }

                if (modifiedUser.IsManager)
            {
                if (!await _userManager.IsInRoleAsync(existentUser, "Manager"))
                {
                    await _userManager.AddToRoleAsync(existentUser, "Manager");
                    message.Append($"The user <strong>{existentUser.Email}</strong> is now a <strong>Manager</strong>!<br/>");
                    //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is now a <strong>Manager</strong>!<br/>";
                }
                else
                {
                    message.Append($"The user <strong>{existentUser.Email}</strong> is still a <strong>Manager</strong>!<br/>");
                    //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still a <strong>Manager</strong>!<br/>";
                }
            }
            else
            {
                if (await _userManager.IsInRoleAsync(existentUser, "Manager"))
                {
                    var company = await _context.PublisherManagers.FirstOrDefaultAsync(i => i.ManagerId == id);
                    if (company is null)
                    {
                        await _userManager.RemoveFromRolesAsync(existentUser, new string[] { "Manager" });
                        message.Append($"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Manager</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Manager</strong>!<br/>";
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Manager</strong> role, as they are still associated with a publisher!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Manager</strong> role, as they are still associated with a publisher!<br/>";
                    }
                }
                else
                {
                    message.Append($"The user <strong>{existentUser.Email}</strong> is still not a <strong>Manager</strong>!<br/>");
                    //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still not a <strong>Manager</strong>!<br/>";
                }
            }

                if (modifiedUser.IsCritic)
                {
                    if (!await _userManager.IsInRoleAsync(existentUser, "Critic"))
                    {
                        await _userManager.AddToRoleAsync(existentUser, "Critic");
                        message.Append($"The user <strong>{existentUser.Email}</strong> is now a <strong>Critic</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is now a <strong>Critic</strong>!<br/>";
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still a <strong>Critic</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still a <strong>Critic</strong>!<br/>";
                    }
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(existentUser, "Critic"))
                    {
                        var company = await _context.PublisherManagers.FirstOrDefaultAsync(i => i.ManagerId == id);
                        if (company is null)
                        {
                            await _userManager.RemoveFromRolesAsync(existentUser, new string[] { "Critic" });
                            message.Append($"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Critic</strong>!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Critic</strong>!<br/>";
                        }
                        else
                        {
                            message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>";
                        }
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still not a <strong>Critic</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still not a <strong>Critic</strong>!<br/>";
                    }
                }

                if (modifiedUser.IsDelivery)
                {
                    if (!await _userManager.IsInRoleAsync(existentUser, "Delivery"))
                    {
                        await _userManager.AddToRoleAsync(existentUser, "Delivery");
                        message.Append($"The user <strong>{existentUser.Email}</strong> is now a <strong>Delivery worker</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is now a <strong>Critic</strong>!<br/>";
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still a <strong>Delivery worker</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still a <strong>Critic</strong>!<br/>";
                    }
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(existentUser, "Delivery"))
                    {
                        var order = await _context.Orders.FirstOrDefaultAsync(i => i.DeliveryId == id && i.IsFinal==false);
                        var schedule = await _context.EmployeeSchedules.FirstOrDefaultAsync(i => i.EmployeeId == id);
                        if (order is null && schedule is null)
                        {
                            await _userManager.RemoveFromRolesAsync(existentUser, new string[] { "Delivery" });
                            message.Append($"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Delivery worker</strong>!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Critic</strong>!<br/>";
                        }
                        else
                        {if(order is not null && schedule is not null)
                            message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Delivery worker</strong> role, as they are still associated with undelivered orders and a schedule!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>";
                         if(order is not null && schedule is null)
                            message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Delivery worker</strong> role, as they are still associated with undelivered orders!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>";
                         if(order is null && schedule is not null)
                            message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Delivery worker</strong> role, as they are still associated with a schedule!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>";
                        }
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still not a <strong>Delivery worker</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still not a <strong>Critic</strong>!<br/>";
                    }
                }

                if (modifiedUser.IsCashier)
                {
                    if (!await _userManager.IsInRoleAsync(existentUser, "Cashier"))
                    {
                        await _userManager.AddToRoleAsync(existentUser, "Cashier");
                        message.Append($"The user <strong>{existentUser.Email}</strong> is now a <strong>Cashier</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> became an <strong>Administrator</strong>!<br/>";
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still a <strong>Cashier</strong>!<br/>");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still an <strong>Administrator</strong>!";
                    }
                }
                else
                {
                    if (await _userManager.IsInRoleAsync(existentUser, "Cashier"))
                    {
                        //var order = await _context.Orders.FirstOrDefaultAsync(i => i.DeliveryId == id && i.IsFinal == false);
                        var schedule = await _context.EmployeeSchedules.FirstOrDefaultAsync(i => i.EmployeeId == id);
                        if (schedule is null)
                        {
                            await _userManager.RemoveFromRolesAsync(existentUser, new string[] { "Cashier" });
                            message.Append($"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Cashier</strong>!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is no longer a <strong>Critic</strong>!<br/>";
                        }
                        else
                        {   /*
                            if (order is not null && schedule is not null)
                                message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Cashier</strong> role, as they are still associated with undelivered orders and a schedule!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>";
                            if (order is not null && schedule is null)
                                message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Cashier</strong> role, as they are still associated with undelivered orders!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>";
                            if (schedule is not null)
                            */
                            message.Append($"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Cashier</strong> role, as they are still associated with a schedule!<br/>");
                            //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> cannot be revoked of their <strong>Critic</strong> role, as they are still associated with a reviewer!<br/>";

                        }
                    }
                    else
                    {
                        message.Append($"The user <strong>{existentUser.Email}</strong> is still not a <strong>Cashier</strong>!");
                        //TempData["MessageUser"] = $"The user <strong>{existentUser.Email}</strong> is still not an <strong>Administrator</strong>!";
                    }
                }

            }
            TempData["MessageUser"] = message.ToString();

            var currentUser = _userManager.Users.FirstOrDefault(u => u.Email == User.Identity!.Name);
            if (currentUser!.Id == existentUser.Id)
            {
                // refresh (of roles) for current user
                await _signInManager.RefreshSignInAsync(currentUser!);
            }
            return RedirectToAction(nameof(Index));
        }
        return View(modifiedUser);
    }
}
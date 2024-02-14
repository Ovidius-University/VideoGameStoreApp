using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Areas.Admin.Models.DTOs;
namespace AppUI.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class RoleController : Controller
{
    public readonly IMapper _mapper;
    public readonly ApplicationDbContext _context;
    public readonly UserManager<AppUser> _userManager;
    public readonly RoleManager<AppRole> _roleManager;

    public RoleController(IMapper mapper, ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<IActionResult> Index()
    {
        var roles = _mapper.Map<List<ExistentRole>>(await _roleManager.Roles.ToListAsync());
        return View(roles);
    }

    // GET: Admin/Role/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Roles == null)
        {
            return NotFound();
        }

        var ExistentRole = _mapper.Map<ExistentRole>(await _context.Roles.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentRole == null)
        {
            return NotFound();
        }

        return View(ExistentRole);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewRoleDto newRole)
    {
        if (ModelState.IsValid)
        {
            if (!await _roleManager.RoleExistsAsync(newRole.Name))
            {
                await _roleManager.CreateAsync(new AppRole() { Name = newRole.Name });
                TempData["MessageRole"] = $"Role <strong>{newRole.Name}</strong> was successfully added!";
            }
            else
            {
                TempData["MessageRole"] = $"Role <strong>{newRole.Name}</strong> already exists!";
            }
            return RedirectToAction(nameof(Index));
        }
        return View(newRole);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var existentRole = _mapper.Map<ExistentRole>(await _roleManager.Roles.FirstOrDefaultAsync(i => i.Id == id));
        if (existentRole == null)
            return NotFound();
        return View(existentRole);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentRole modifiedRole)
    {
        var existentRole = await _roleManager.Roles.FirstOrDefaultAsync(i => i.Id == id);
        if (existentRole is null)
            return NotFound();
        if (existentRole.Id != id)
            return BadRequest();
        if (ModelState.IsValid)
        {
            var users = await _userManager.GetUsersInRoleAsync(existentRole.Name!);
            if (users.Count > 0)
            {
                TempData["MessageRole"] = $"Role <strong>{existentRole.Name}</strong> can not be edited as it has {users.Count} users!";
            }
            else if (!await _roleManager.RoleExistsAsync(modifiedRole.Name))
            {
                existentRole.Name = modifiedRole.Name;
                await _roleManager.UpdateAsync(existentRole);
                TempData["MessageRole"] = $"Role <strong>{modifiedRole.Name}</strong> was successfully modified!";
            }
            else
            {
                TempData["MessageRole"] = $"Role <strong>{modifiedRole.Name}</strong> already exists!";
            }
            return RedirectToAction(nameof(Index));
        }
        return View(modifiedRole);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var existentRole = _mapper.Map<ExistentRole>(await _roleManager.Roles.FirstOrDefaultAsync(i => i.Id == id));
        if (existentRole == null)
            return NotFound();
        return View(existentRole);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> ConfirmDelete(int id)
    {
        var existentRole = await _roleManager.Roles.FirstOrDefaultAsync(i => i.Id == id);
        if (existentRole == null)
            return NotFound();
        if (ModelState.IsValid)
        {
            var users = await _userManager.GetUsersInRoleAsync(existentRole.Name!);
            if (users.Count > 0)
            {
                TempData["MessageRole"] = $"Role <strong>{existentRole.Name}</strong> can not be deleted as it has {users.Count} users!";
            }
            else
            {
                await _roleManager.DeleteAsync(existentRole);
                TempData["MessageRole"] = $"Role <strong>{existentRole.Name}</strong> was deleted!";
            }
            return RedirectToAction(nameof(Index));
        }
        return View(_mapper.Map<ExistentRole>(existentRole));
    }
}
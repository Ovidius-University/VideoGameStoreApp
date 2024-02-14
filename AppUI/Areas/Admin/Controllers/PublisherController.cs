using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
namespace AppUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class PublisherController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public PublisherController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/Publisher
    public async Task<IActionResult> Index()
    {
        if (_context.Publishers == null)
            return Problem("Entity set 'ApplicationDbContext.Publishers' is null.");
        return View(_mapper.Map<List<ExistentPublisherDto>>(await _context.Publishers.ToListAsync()));
    }

    // GET: Admin/Publisher/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Publishers == null)
        {
            return NotFound();
        }

        var ExistentPublisher = _mapper.Map<ExistentPublisherDto>(await _context.Publishers.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentPublisher == null)
        {
            return NotFound();
        }

        return View(ExistentPublisher);
    }

    // GET: Admin/Publisher/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Publisher/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] NewPublisherDto newPublisher)
    {
        if (ModelState.IsValid)
        {
            var history = await _context.Publishers
                .Where(i => i.Name == newPublisher.Name)
                .FirstOrDefaultAsync();
            if (history != null)
            {
                TempData["MessagePublisher"] = $"The publisher <strong>{newPublisher.Name}</strong> is already added!";
                return RedirectToAction(nameof(Index));
            }

            var publisher = _mapper.Map<Publisher>(newPublisher);
            _context.Add(publisher);
            await _context.SaveChangesAsync();
            TempData["MessagePublisher"] = $"We added publisher <strong>{newPublisher.Name}</strong>!";
            return RedirectToAction(nameof(Index));
        }
        return View(newPublisher);
    }

    // GET: Admin/Publisher/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Publishers == null)
        {
            return NotFound();
        }

        var ExistentPublisher = _mapper.Map<ExistentPublisherDto>(await _context.Publishers.FindAsync(id));
        if (ExistentPublisher == null)
        {
            return NotFound();
        }
        return View(ExistentPublisher);
    }

    // POST: Admin/Publisher/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentPublisherDto modifiedPublisher)
    {
        if (id != modifiedPublisher.Id)
        {
            return NotFound();
        }
        var ExistentPublisher = await _context.Publishers.FindAsync(id);
        if (ExistentPublisher == null)
        {
            return NotFound();
        }
        if (ExistentPublisher.Id != modifiedPublisher.Id)
        {
            return BadRequest();
        }
        var history = await _context.Publishers
                .Where(i => i.Name == modifiedPublisher.Name && i.Id!=id)
                .FirstOrDefaultAsync();
        if (history != null)
        {
            TempData["MessagePublisher"] = $"The publisher <strong>{modifiedPublisher.Name}</strong> already exists!";
            return RedirectToAction(nameof(Index));
        }
        if (ModelState.IsValid)
        {
            try
            {
                ExistentPublisher.Name = modifiedPublisher.Name;
                _context.Update(ExistentPublisher);
                TempData["MessagePublisher"] = $"We modified publisher <strong>{ExistentPublisher.Name}</strong> in <strong>{modifiedPublisher.Name}</strong>!";
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(ExistentPublisher.Id))
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
        return View(modifiedPublisher);
    }

    // GET: Admin/Publisher/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Publishers == null)
        {
            return NotFound();
        }

        var ExistentPublisher = _mapper.Map<ExistentPublisherDto>(await _context.Publishers.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentPublisher == null)
        {
            return NotFound();
        }

        return View(ExistentPublisher);
    }

    // POST: Admin/Publisher/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Publishers == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Publishers' is null.");
        }
        var ExistentPublisher = await _context.Publishers
            .Include(t => t.VideoGames)
            .Include(t => t.Managers)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentPublisher == null)
        {
            return NotFound();
        }
        else
        {
            if (ExistentPublisher.VideoGames!.Count == 0 && ExistentPublisher.Managers!.Count == 0)
            {
                _context.Publishers.Remove(ExistentPublisher);
                TempData["MessagePublisher"] = $"Publisher <strong>{ExistentPublisher.Name}</strong> was successfully deleted!";
            }
            else
            {
                TempData["MessagePublisher"] = $"Publisher <strong>{ExistentPublisher.Name}</strong> can not be deleted as it has associated video games or managers!";
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Publisher/Edit/5
    public async Task<IActionResult> Managers(int id)
    {
        if (_context.Publishers == null)
        {
            return NotFound();
        }

        var ExistentPublisher = await _context.Publishers
            .Include(t => t.Managers)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentPublisher is null)
        {
            return NotFound();
        }
        var Managers = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Manager"));

        var associatedManagers = await _context.PublisherManagers.Where(c => c.PublisherId == id).ToListAsync();

        for (var i = Managers.Count - 1; i >= 0; i--)
        {
            if (!associatedManagers.Any(m => m.ManagerId == Managers[i].Id))
            {
                Managers.Remove(Managers[i]);
            }
        }
        var output = new PublisherManagersVm()
        {
            Id = id,
            Name = ExistentPublisher.Name,
            listManagers = Managers
        };
        return View(output);
    }

    public async Task<IActionResult> AddManager(int id)
    {
        if (_context.Publishers == null)
        {
            return NotFound();
        }

        var ExistentPublisher = await _context.Publishers
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentPublisher is null)
        {
            return NotFound();
        }

        var Managers = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Manager"));
        ViewBag.Managers = new SelectList(Managers, "Id", "Email");
        return View(new PublisherManagerDto()
        {
            PublisherId = id,
            Publisher = ExistentPublisher.Name
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddManager([FromRoute] int id, [FromForm] PublisherManagerDto userPublisher)
    {
        if (_context.Publishers == null)
        {
            return NotFound();
        }

        var ExistentPublisher = await _context.Publishers
            .Include(t => t.Managers)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentPublisher is null)
        {
            return NotFound();
        }
        
        if (ModelState.IsValid)
        {
            //var verify = _context.PublisherManagers.Where(q => q.PublisherId == id).FirstOrDefault();
            //if (verify != null) { TempData["MessagePublisherManager"] = "Publisher already has a manager!"; }

            var verify = _context.PublisherManagers.Where(q => q.PublisherId != id && q.ManagerId == userPublisher.ManagerId).FirstOrDefault();
            if (verify != null) 
            { 
                TempData["MessagePublisherManager"] = "Manager is already associated with a different publisher!";
                return RedirectToAction(nameof(AddManager), new { id });
            }
            verify = null;

            var existentManager = _context.PublisherManagers.Where(q => q.PublisherId == id && q.ManagerId == userPublisher.ManagerId).FirstOrDefault();
            if (existentManager is null)
            {
                var publisherManager = new PublisherManager()
                {
                    PublisherId = id,
                    ManagerId = userPublisher.ManagerId
                };
                await _context.PublisherManagers.AddAsync(publisherManager);
                await _context.SaveChangesAsync();
                TempData["MessagePublisherManager"] = "Manager was associated with the publisher!";
            }
            else
            {
                TempData["MessagePublisherManager"] = "Manager is already associated with the publisher!";
            }
            return RedirectToAction(nameof(AddManager), new { id });
        }
        var Managers = _mapper.Map<List<ExistentUserDto>>(await _userManager.GetUsersInRoleAsync("Manager"));
        ViewBag.Managers = new SelectList(Managers, "Id", "Email", userPublisher.ManagerId);
        return View(userPublisher);
    }

    // GET: Admin/Publisher/DeleteManager/5
    public async Task<IActionResult> DeleteManager(int? id, int? pub, string? email)
    {
        if (id == null || pub == null || email == null || _context.PublisherManagers == null || _context.Publishers == null)
        {
            return NotFound();
        }

        var ExistentManager = await _context.PublisherManagers
            .FirstOrDefaultAsync(i => i.ManagerId == id && i.PublisherId == pub); 
        if (ExistentManager == null)
        {
            return NotFound();
        }

        var BasicManager = await _userManager.FindByEmailAsync(email);
        ExistentManagerDto managerDto = new();
        if (BasicManager == null || BasicManager.Email==null)
        {
            return NotFound();
        }
        managerDto.Id = BasicManager.Id;
        managerDto.Email = BasicManager.Email;

        return View(managerDto);
    }

    // POST: Admin/Publisher/DeleteManagerConfirmed/5
    [HttpPost, ActionName("DeleteManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteManagerConfirmed(int id)
    {
        if (_context.PublisherManagers == null)
        {
            return Problem("Entity set 'ApplicationDbContext.PublisherManagers' is null.");
        }
        var ExistentManager = await _context.PublisherManagers
            .FirstOrDefaultAsync(i => i.ManagerId == id);
        var manager = await _userManager.Users.FirstOrDefaultAsync(i => i.Id == id);
        if (ExistentManager == null || manager == null)
        {
            return NotFound();
        }
        else
        {
            _context.PublisherManagers.Remove(ExistentManager);
            TempData["MessagePublisher"] = $"Manager <strong>{manager.UserName}</strong> was successfully disassociated from the publisher!";
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PublisherExists(int id)
    {
        return (_context.Publishers?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
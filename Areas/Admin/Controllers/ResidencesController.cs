using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;
using AirBB.Models.DataLayer;
using AirBB.Models.DataLayer.Repositories;
using AirBB.Models.Domain;
using System.Linq.Expressions;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResidencesController : Controller
    {
        private readonly IGenericRepository<Residence> _resRepo;
        private readonly IGenericRepository<Location> _locRepo;
        private readonly IGenericRepository<User> _userRepo;

        public ResidencesController(IGenericRepository<Residence> resRepo,
            IGenericRepository<Location> locRepo,
            IGenericRepository<User> userRepo)
        {
            _resRepo = resRepo;
            _locRepo = locRepo;
            _userRepo = userRepo;
        }

        // GET: Admin/Residences
        public async Task<IActionResult> Index()
        {
            var options = new QueryOptions<Residence>
            {
                Includes = new List<Expression<Func<Residence, object>>> { r => r.Location!, r => r.Owner! }
            };

            var residences = await _resRepo.GetAllAsync(options);
            return View(residences);
        }

        // GET: Admin/Residences/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Locations = await _locRepo.GetAllAsync();
            var owners = (await _userRepo.GetAllAsync()).Where(u => u.UserType == UserType.Owner).ToList();
            ViewBag.Owners = owners;
            return View();
        }

        // POST: Admin/Residences/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResidenceId,Name,ResidencePicture,LocationId,OwnerId,GuestNumber,BedroomNumber,BathroomNumber,BuiltYear,PricePerNight")] Residence residence)
        {
            // Log ModelState errors for debugging
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
            }

            // Check that OwnerId exists in Users table
            var ownerExists = await _userRepo.AnyAsync(u => u.UserId == residence.OwnerId);
            if (!ownerExists)
            {
                ModelState.AddModelError("OwnerId", "Owner (User) does not exist. Please select a valid owner.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _resRepo.AddAsync(residence);
                    TempData["SuccessMessage"] = "Residence created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during Create: {ex.Message}");
                    ModelState.AddModelError("", $"Error creating residence: {ex.Message}");
                }
            }

            ViewBag.Locations = await _locRepo.GetAllAsync();
            ViewBag.Owners = (await _userRepo.GetAllAsync()).Where(u => u.UserType == UserType.Owner).ToList();
            return View(residence);
        }

        // GET: Admin/Residences/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residence = await _resRepo.GetByIdAsync(id);
            if (residence == null)
            {
                return NotFound();
            }

            ViewBag.Locations = await _locRepo.GetAllAsync();
            ViewBag.Owners = (await _userRepo.GetAllAsync()).Where(u => u.UserType == UserType.Owner).ToList();
            return View(residence);
        }

        // POST: Admin/Residences/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResidenceId,Name,ResidencePicture,LocationId,OwnerId,GuestNumber,BedroomNumber,BathroomNumber,BuiltYear,PricePerNight")] Residence residence)
        {
            if (id != residence.ResidenceId)
            {
                return NotFound();
            }

            // Log ModelState errors for debugging
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _resRepo.UpdateAsync(residence);
                    TempData["SuccessMessage"] = "Residence updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResidenceExists(residence.ResidenceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception during Edit: {ex.Message}");
                    ModelState.AddModelError("", $"Error updating residence: {ex.Message}");
                }
            }

            ViewBag.Locations = await _locRepo.GetAllAsync();
            ViewBag.Owners = (await _userRepo.GetAllAsync()).Where(u => u.UserType == UserType.Owner).ToList();
            return View(residence);
        }

        // GET: Admin/Residences/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var options = new QueryOptions<Residence>
            {
                Filter = r => r.ResidenceId == id,
                Includes = new List<Expression<Func<Residence, object>>> { r => r.Location!, r => r.Owner! }
            };

            var residence = (await _resRepo.GetAllAsync(options)).FirstOrDefault();
            if (residence == null)
            {
                return NotFound();
            }

            return View(residence);
        }

        // POST: Admin/Residences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var residence = await _resRepo.GetByIdAsync(id);
            if (residence != null)
            {
                await _resRepo.DeleteAsync(residence);
                TempData["SuccessMessage"] = "Residence deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ResidenceExists(int id)
        {
            return _resRepo.Query().Any(e => e.ResidenceId == id);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AirBB.Models;
using AirBB.Models.DataLayer.Repositories;
using AirBB.Models.DataLayer;
using AirBB.Models.Domain;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly IGenericRepository<User> _userRepo;

        public UsersController(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _userRepo.GetAllAsync();
            return View(users);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Name,PhoneNumber,Email,SSN,UserType,DOB")] User user)
        {
            // Custom validation: Either PhoneNumber or Email must be present
            if (string.IsNullOrEmpty(user.PhoneNumber) && string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError(string.Empty, "Either Phone Number or Email must be provided as a means of contact.");
            }

            if (ModelState.IsValid)
            {
                await _userRepo.AddAsync(user);
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }

            if (!ViewData.ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the validation errors.";
            }

            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,PhoneNumber,Email,SSN,UserType,DOB")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            // Custom validation: Either PhoneNumber or Email must be present
            if (string.IsNullOrEmpty(user.PhoneNumber) && string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError(string.Empty, "Either Phone Number or Email must be provided as a means of contact.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userRepo.UpdateAsync(user);
                    TempData["SuccessMessage"] = "User updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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

            if (!ViewData.ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fix the validation errors.";
            }

            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var options = new QueryOptions<User>
            {
                Filter = u => u.UserId == id
            };
            var user = (await _userRepo.GetAllAsync(options)).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepo.DeleteAsync(user);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _userRepo.Query().Any(e => e.UserId == id);
        }
    }
}
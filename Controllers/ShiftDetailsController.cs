using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Data;
using ShiftSchedularApplication.Models;
using Microsoft.AspNetCore.Authorization;

namespace ShiftSchedularApplication.Controllers
{
    [Authorize]
    public class ShiftDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShiftDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShiftDetails
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var shiftDetails = await _context.ShiftDetails
                .Include(sd => sd.Shift)
                .ToListAsync();
            return View(shiftDetails);
        }

        // GET: ShiftDetails/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftDetail = await _context.ShiftDetails
                .Include(sd => sd.Shift)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftDetail == null)
            {
                return NotFound();
            }

            return View(shiftDetail);
        }

        // GET: ShiftDetails/Create
        public IActionResult Create()
        {
            ViewData["ShiftId"] = new SelectList(_context.Shifts, "Id", "Id");
            return View();
        }

        // POST: ShiftDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ShiftId,TaskDescription,TaskStartTime,TaskEndTime,TaskType,Notes,IsCompleted")] ShiftDetail shiftDetail)
        {
            if (ModelState.IsValid)
            {
                // Verify the shift belongs to the current user
                var shift = await _context.Shifts.FindAsync(shiftDetail.ShiftId);
                if (shift == null)
                {
                    ModelState.AddModelError("ShiftId", "Invalid shift selected.");
                    ViewData["ShiftId"] = new SelectList(_context.Shifts, "Id", "Id", shiftDetail.ShiftId);
                    return View(shiftDetail);
                }

                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId) || shift.EmployeeId != currentUserId)
                {
                    return Forbid();
                }

                _context.Add(shiftDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShiftId"] = new SelectList(_context.Shifts, "Id", "Id", shiftDetail.ShiftId);
            return View(shiftDetail);
        }

        // GET: ShiftDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftDetail = await _context.ShiftDetails
                .Include(sd => sd.Shift)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftDetail == null)
            {
                return NotFound();
            }

            // Ensure user can only edit details for their own shifts
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || shiftDetail.Shift?.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            ViewData["ShiftId"] = new SelectList(_context.Shifts, "Id", "Id", shiftDetail.ShiftId);
            return View(shiftDetail);
        }

        // POST: ShiftDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShiftId,TaskDescription,TaskStartTime,TaskEndTime,TaskType,Notes,IsCompleted")] ShiftDetail shiftDetail)
        {
            if (id != shiftDetail.Id)
            {
                return NotFound();
            }

            // Ensure user can only edit details for their own shifts
            var shift = await _context.Shifts.FindAsync(shiftDetail.ShiftId);
            if (shift == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || shift.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shiftDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftDetailExists(shiftDetail.Id))
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
            ViewData["ShiftId"] = new SelectList(_context.Shifts, "Id", "Id", shiftDetail.ShiftId);
            return View(shiftDetail);
        }

        // GET: ShiftDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftDetail = await _context.ShiftDetails
                .Include(sd => sd.Shift)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftDetail == null)
            {
                return NotFound();
            }

            // Ensure user can only delete details for their own shifts
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || shiftDetail.Shift?.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            return View(shiftDetail);
        }

        // POST: ShiftDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shiftDetail = await _context.ShiftDetails
                .Include(sd => sd.Shift)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shiftDetail != null)
            {
                // Ensure user can only delete details for their own shifts
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId) || shiftDetail.Shift?.EmployeeId != currentUserId)
                {
                    return Forbid();
                }

                _context.ShiftDetails.Remove(shiftDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftDetailExists(int id)
        {
            return _context.ShiftDetails.Any(e => e.Id == id);
        }
    }
}

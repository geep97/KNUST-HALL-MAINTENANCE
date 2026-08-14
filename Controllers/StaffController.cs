using System.Security.Claims;
using KnustHallMaintenance.Models;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

// Staff and Maintenance share the same dashboard — permissions are gated in the views/actions, not by separate screens.
[Authorize(Roles = "Staff,Maintenance")]
public class StaffController : Controller
{
    private readonly IComplaintService _complaintService;
    private readonly IUserService _userService;

    public StaffController(IComplaintService complaintService, IUserService userService)
    {
        _complaintService = complaintService;
        _userService = userService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Screen 5: Staff Dashboard
    public async Task<IActionResult> Index(ComplaintStatus? status, ComplaintCategory? category)
    {
        var isMaintenance = User.IsInRole(nameof(UserRole.Maintenance));
        var complaints = isMaintenance
            ? await _complaintService.GetAssignedToAsync(CurrentUserId, status, category)
            : await _complaintService.GetAllAsync(status, category);

        ViewBag.StatusFilter = status;
        ViewBag.CategoryFilter = category;
        ViewBag.IsMaintenance = isMaintenance;
        ViewBag.SubmittedCount = complaints.Count(c => c.Status == ComplaintStatus.Submitted);
        ViewBag.AssignedCount = complaints.Count(c => c.Status == ComplaintStatus.Assigned);
        ViewBag.InProgressCount = complaints.Count(c => c.Status == ComplaintStatus.InProgress);
        ViewBag.ResolvedCount = complaints.Count(c => c.Status == ComplaintStatus.Resolved);
        return View(complaints);
    }

    // Screen 6: Complaint Details / Assignment (staff view)
    public async Task<IActionResult> Details(int id)
    {
        var complaint = await _complaintService.GetByIdAsync(id);
        if (complaint == null) return NotFound();

        if (User.IsInRole(nameof(UserRole.Maintenance)) && complaint.AssignedToId != CurrentUserId)
            return Forbid();

        ViewBag.IsMaintenance = User.IsInRole(nameof(UserRole.Maintenance));
        ViewBag.MaintenanceStaff = await _userService.GetMaintenanceStaffAsync();

        return View(complaint);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> Assign(int complaintId, int assignToId)
    {
        await _complaintService.AssignAsync(complaintId, CurrentUserId, assignToId);
        return RedirectToAction(nameof(Details), new { id = complaintId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Maintenance")]
    public async Task<IActionResult> UpdateStatus(int complaintId, ComplaintStatus status, string? note)
    {
        if (status == ComplaintStatus.Submitted)
            return BadRequest();

        var updated = await _complaintService.UpdateStatusAsync(complaintId, status, note, CurrentUserId);
        if (!updated) return Forbid();

        TempData["Success"] = "Work order updated.";
        return RedirectToAction(nameof(Details), new { id = complaintId });
    }
}

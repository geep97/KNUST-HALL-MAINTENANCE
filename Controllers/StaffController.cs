using System.Security.Claims;
using KnustHallMaintenance.Models;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

[Authorize(Roles = "HallStaff,Maintenance")]
public class StaffController : Controller
{
    private readonly IComplaintService _complaintService;
    private readonly IUserService _userService;
    private readonly ComplaintSlaService _slaService;

    public StaffController(
        IComplaintService complaintService,
        IUserService userService,
        ComplaintSlaService slaService)
    {
        _complaintService = complaintService;
        _userService = userService;
        _slaService = slaService;
    }

    private int CurrentUserId =>
        int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

    public async Task<IActionResult> Index(
        ComplaintStatus? status,
        ComplaintCategory? category)
    {
        var isMaintenance =
            User.IsInRole(nameof(UserRole.Maintenance));

        List<Complaint> complaints;

        if (isMaintenance)
        {
            complaints =
                await _complaintService.GetAssignedToAsync(
                    CurrentUserId,
                    status,
                    category);
        }
        else
        {
            var hall =
                User.FindFirstValue("Hall");

            if (string.IsNullOrWhiteSpace(hall))
            {
                return Forbid();
            }

            complaints =
                await _complaintService.GetForHallAsync(
                    hall,
                    status,
                    category);
        }

        ViewBag.StatusFilter = status;
        ViewBag.CategoryFilter = category;
        ViewBag.IsMaintenance = isMaintenance;

        ViewBag.SubmittedCount =
            complaints.Count(
                c => c.Status == ComplaintStatus.Submitted);

        ViewBag.AssignedCount =
            complaints.Count(
                c => c.Status == ComplaintStatus.Assigned);

        ViewBag.InProgressCount =
            complaints.Count(
                c => c.Status == ComplaintStatus.InProgress);

        ViewBag.ResolvedCount =
            complaints.Count(
                c => c.Status == ComplaintStatus.Resolved);

        ViewBag.OverdueCount =
            complaints.Count(
                c => _slaService.IsOverdue(c));

        ViewBag.NeedsAttentionCount =
            complaints.Count(
                c => _slaService.NeedsAttention(c));

        var overdueComplaints =
            complaints
                .Where(c => _slaService.IsOverdue(c))
                .OrderBy(c => c.DueAt)
                .ToList();

        var attentionComplaints =
            complaints
                .Where(c =>
                    !_slaService.IsOverdue(c) &&
                    _slaService.NeedsAttention(c))
                .OrderBy(c => c.LastActivityAt)
                .ToList();

        ViewBag.OverdueComplaints =
            overdueComplaints;

        ViewBag.AttentionComplaints =
            attentionComplaints;

        foreach (var complaint in complaints)
        {
            ViewData[$"Overdue_{complaint.Id}"] =
                _slaService.IsOverdue(complaint);

            ViewData[$"NeedsAttention_{complaint.Id}"] =
                _slaService.NeedsAttention(complaint);
        }

        return View(complaints);
    }

    public async Task<IActionResult> Details(int id)
    {
        var complaint =
            await _complaintService.GetByIdAsync(id);

        if (complaint == null)
        {
            return NotFound();
        }

        if (User.IsInRole(nameof(UserRole.Maintenance)) &&
            complaint.AssignedToId != CurrentUserId)
        {
            return Forbid();
        }

        if (User.IsInRole(nameof(UserRole.HallStaff)))
        {
            var hall =
                User.FindFirstValue("Hall");

            if (string.IsNullOrWhiteSpace(hall) ||
                !string.Equals(
                    hall,
                    complaint.Hall,
                    StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }
        }

        ViewBag.IsMaintenance =
            User.IsInRole(nameof(UserRole.Maintenance));

        ViewBag.IsOverdue =
            _slaService.IsOverdue(complaint);

        ViewBag.NeedsAttention =
            _slaService.NeedsAttention(complaint);

        ViewBag.MaintenanceStaff =
            await _userService.GetMaintenanceStaffAsync(
                complaint.Hall);

        return View(complaint);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "HallStaff")]
    public async Task<IActionResult> Assign(
        int complaintId,
        int assignToId)
    {
        var assigned =
            await _complaintService.AssignAsync(
                complaintId,
                CurrentUserId,
                assignToId);

        if (!assigned)
        {
            return Forbid();
        }

        TempData["Success"] =
            "Maintenance request assigned successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = complaintId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Maintenance")]
    public async Task<IActionResult> UpdateStatus(
        int complaintId,
        ComplaintStatus status,
        string? note)
    {
        if (status == ComplaintStatus.Submitted)
        {
            return BadRequest();
        }

        var updated =
            await _complaintService.UpdateStatusAsync(
                complaintId,
                status,
                note,
                CurrentUserId);

        if (!updated)
        {
            return Forbid();
        }

        TempData["Success"] =
            "Work order updated.";

        return RedirectToAction(
            nameof(Details),
            new { id = complaintId });
    }
}
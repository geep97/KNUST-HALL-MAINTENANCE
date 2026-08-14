using System.Security.Claims;
using KnustHallMaintenance.Models;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;


// =========================================================
// STAFF + MAINTENANCE CONTROLLER
// =========================================================
//
// Hall Staff and Maintenance personnel share this dashboard.
//
// Hall Staff:
// - See complaints belonging to their hall
// - Open complaint details
// - Assign complaints to maintenance personnel from their hall
//
// Maintenance:
// - See only work orders assigned to them
// - Open assigned complaint details
// - Update work-order status
//
// =========================================================

[Authorize(Roles = "HallStaff,Maintenance")]
public class StaffController : Controller
{
    private readonly IComplaintService _complaintService;
    private readonly IUserService _userService;


    public StaffController(
        IComplaintService complaintService,
        IUserService userService)
    {
        _complaintService = complaintService;
        _userService = userService;
    }


    // =========================================================
    // CURRENT USER ID
    // =========================================================

    private int CurrentUserId =>
        int.Parse(
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            )!
        );


    // =========================================================
    // STAFF / MAINTENANCE DASHBOARD
    // =========================================================

    public async Task<IActionResult> Index(
        ComplaintStatus? status,
        ComplaintCategory? category)
    {
        var isMaintenance =
            User.IsInRole(
                nameof(UserRole.Maintenance)
            );


        List<Complaint> complaints;


        // =====================================================
        // MAINTENANCE PERSONNEL
        // =====================================================
        //
        // Maintenance personnel only see work orders assigned
        // directly to them.
        // =====================================================

        if (isMaintenance)
        {
            complaints =
                await _complaintService.GetAssignedToAsync(
                    CurrentUserId,
                    status,
                    category
                );
        }


        // =====================================================
        // HALL STAFF
        // =====================================================
        //
        // Hall Staff only see complaints belonging to their
        // assigned hall.
        // =====================================================

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
                    category
                );
        }


        // =====================================================
        // DASHBOARD FILTERS
        // =====================================================

        ViewBag.StatusFilter =
            status;

        ViewBag.CategoryFilter =
            category;

        ViewBag.IsMaintenance =
            isMaintenance;


        // =====================================================
        // DASHBOARD COUNTS
        // =====================================================

        ViewBag.SubmittedCount =
            complaints.Count(
                c =>
                    c.Status ==
                    ComplaintStatus.Submitted
            );


        ViewBag.AssignedCount =
            complaints.Count(
                c =>
                    c.Status ==
                    ComplaintStatus.Assigned
            );


        ViewBag.InProgressCount =
            complaints.Count(
                c =>
                    c.Status ==
                    ComplaintStatus.InProgress
            );


        ViewBag.ResolvedCount =
            complaints.Count(
                c =>
                    c.Status ==
                    ComplaintStatus.Resolved
            );


        return View(
            complaints
        );
    }


    // =========================================================
    // COMPLAINT DETAILS
    // =========================================================

    public async Task<IActionResult> Details(
        int id)
    {
        var complaint =
            await _complaintService.GetByIdAsync(
                id
            );


        if (complaint == null)
        {
            return NotFound();
        }


        // =====================================================
        // MAINTENANCE ACCESS
        // =====================================================
        //
        // Maintenance personnel can only open complaints
        // assigned to themselves.
        // =====================================================

        if (
            User.IsInRole(
                nameof(UserRole.Maintenance)
            )
            &&
            complaint.AssignedToId !=
            CurrentUserId
        )
        {
            return Forbid();
        }


        // =====================================================
        // HALL STAFF ACCESS
        // =====================================================
        //
        // Hall Staff can only open complaints belonging to
        // their own hall.
        // =====================================================

        if (
            User.IsInRole(
                nameof(UserRole.HallStaff)
            )
        )
        {
            var hall =
                User.FindFirstValue("Hall");


            if (
                string.IsNullOrWhiteSpace(hall)
                ||
                !string.Equals(
                    hall,
                    complaint.Hall,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Forbid();
            }
        }


        ViewBag.IsMaintenance =
            User.IsInRole(
                nameof(UserRole.Maintenance)
            );


        // =====================================================
        // MAINTENANCE STAFF
        // =====================================================
        //
        // Used by Hall Staff when assigning a complaint.
        //
        // The service layer will also verify that the selected
        // technician belongs to the same hall.
        // =====================================================

        ViewBag.MaintenanceStaff =
            await _userService
                .GetMaintenanceStaffAsync();


        return View(
            complaint
        );
    }


    // =========================================================
    // ASSIGN COMPLAINT
    // =========================================================
    //
    // Only Hall Staff can assign complaints.
    //
    // The ComplaintService performs the final security check
    // to ensure:
    //
    // Hall Staff hall == Complaint hall
    //
    // and
    //
    // Technician hall == Complaint hall
    // =========================================================

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
                assignToId
            );


        if (!assigned)
        {
            return Forbid();
        }


        TempData["Success"] =
            "Maintenance request assigned successfully.";


        return RedirectToAction(
            nameof(Details),
            new
            {
                id = complaintId
            }
        );
    }


    // =========================================================
    // UPDATE WORK ORDER STATUS
    // =========================================================
    //
    // Only Maintenance personnel can update status.
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Maintenance")]
    public async Task<IActionResult> UpdateStatus(
        int complaintId,
        ComplaintStatus status,
        string? note)
    {
        // A maintenance technician cannot move
        // a complaint back to Submitted.
        if (
            status ==
            ComplaintStatus.Submitted
        )
        {
            return BadRequest();
        }


        var updated =
            await _complaintService
                .UpdateStatusAsync(
                    complaintId,
                    status,
                    note,
                    CurrentUserId
                );


        if (!updated)
        {
            return Forbid();
        }


        TempData["Success"] =
            "Work order updated.";


        return RedirectToAction(
            nameof(Details),
            new
            {
                id = complaintId
            }
        );
    }
}
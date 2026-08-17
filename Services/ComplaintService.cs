using KnustHallMaintenance.Data;
using KnustHallMaintenance.Models;
using Microsoft.EntityFrameworkCore;

namespace KnustHallMaintenance.Services;

public class ComplaintService : IComplaintService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public ComplaintService(
        ApplicationDbContext context,
        IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Complaint> CreateAsync(
        Complaint complaint)
    {
        complaint.Status =
            ComplaintStatus.Submitted;

        complaint.DateCreated =
            DateTime.Now;

        _context.Complaints.Add(complaint);

        await _context.SaveChangesAsync();

        return complaint;
    }

    public async Task<List<Complaint>>
        GetForStudentAsync(int studentId)
    {
        return await _context.Complaints
            .Where(
                c => c.StudentId == studentId
            )
            .OrderByDescending(
                c => c.DateCreated
            )
            .ToListAsync();
    }

    public async Task<List<Complaint>>
        GetAllAsync(
            ComplaintStatus? statusFilter = null,
            ComplaintCategory? categoryFilter = null)
    {
        var query =
            _context.Complaints
                .Include(c => c.Student)
                .Include(c => c.AssignedTo)
                .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(
                c => c.Status == statusFilter.Value
            );
        }

        if (categoryFilter.HasValue)
        {
            query = query.Where(
                c => c.Category == categoryFilter.Value
            );
        }

        return await query
            .OrderByDescending(
                c => c.DateCreated
            )
            .ToListAsync();
    }

    public async Task<List<Complaint>>
        GetForHallAsync(
            string hall,
            ComplaintStatus? statusFilter = null,
            ComplaintCategory? categoryFilter = null)
    {
        var query =
            _context.Complaints
                .Include(c => c.Student)
                .Include(c => c.AssignedTo)
                .Where(c => c.Hall == hall)
                .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(
                c => c.Status == statusFilter.Value
            );
        }

        if (categoryFilter.HasValue)
        {
            query = query.Where(
                c => c.Category == categoryFilter.Value
            );
        }

        return await query
            .OrderByDescending(
                c => c.DateCreated
            )
            .ToListAsync();
    }

    public async Task<List<Complaint>>
        GetAssignedToAsync(
            int maintenanceId,
            ComplaintStatus? statusFilter = null,
            ComplaintCategory? categoryFilter = null)
    {
        var query =
            _context.Complaints
                .Include(c => c.Student)
                .Include(c => c.AssignedTo)
                .Where(
                    c => c.AssignedToId == maintenanceId
                )
                .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(
                c => c.Status == statusFilter.Value
            );
        }

        if (categoryFilter.HasValue)
        {
            query = query.Where(
                c => c.Category == categoryFilter.Value
            );
        }

        return await query
            .OrderByDescending(
                c => c.DateCreated
            )
            .ToListAsync();
    }

    public async Task<Complaint?> GetByIdAsync(
        int id)
    {
        return await _context.Complaints
            .Include(c => c.Student)
            .Include(c => c.AssignedTo)
            .Include(c => c.Updates)
                .ThenInclude(
                    u => u.UpdatedBy
                )
            .FirstOrDefaultAsync(
                c => c.Id == id
            );
    }

    public async Task<bool> AssignAsync(
        int complaintId,
        int staffId,
        int assignToId)
    {
        var complaint =
            await _context.Complaints
                .FirstOrDefaultAsync(
                    c => c.Id == complaintId
                );

        if (complaint == null)
        {
            return false;
        }

        var staff =
            await _userService.GetByIdAsync(
                staffId
            );

        if (staff == null)
        {
            return false;
        }

        var isHallStaff =
            await _userService.IsInRoleAsync(
                staff,
                UserRole.HallStaff
            );

        if (!isHallStaff)
        {
            return false;
        }

        if (
            string.IsNullOrWhiteSpace(
                staff.Hall
            )
        )
        {
            return false;
        }

        if (
            !string.Equals(
                staff.Hall,
                complaint.Hall,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return false;
        }

        var assignee =
            await _userService.GetByIdAsync(
                assignToId
            );

        if (assignee == null)
        {
            return false;
        }

        var isMaintenance =
            await _userService.IsInRoleAsync(
                assignee,
                UserRole.Maintenance
            );

        if (!isMaintenance)
        {
            return false;
        }

        if (
            string.IsNullOrWhiteSpace(
                assignee.Hall
            )
        )
        {
            return false;
        }

        if (
            !string.Equals(
                assignee.Hall,
                complaint.Hall,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return false;
        }

        complaint.AssignedToId =
            assignToId;

        if (
            complaint.Status ==
            ComplaintStatus.Submitted
        )
        {
            complaint.Status =
                ComplaintStatus.Assigned;
        }

        _context.ComplaintUpdates.Add(
            new ComplaintUpdate
            {
                ComplaintId =
                    complaintId,

                UpdatedById =
                    staffId,

                Note =
                    $"Assigned to {assignee.FullName}.",

                Timestamp =
                    DateTime.Now
            }
        );

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateStatusAsync(
        int complaintId,
        ComplaintStatus status,
        string? note,
        int updatedById)
    {
        var complaint =
            await _context.Complaints
                .FindAsync(complaintId);

        if (
            complaint == null ||
            complaint.AssignedToId != updatedById
        )
        {
            return false;
        }

        complaint.Status =
            status;

        if (
            status ==
            ComplaintStatus.Resolved
        )
        {
            complaint.DateResolved =
                DateTime.Now;
        }
        else
        {
            complaint.DateResolved =
                null;
        }

        _context.ComplaintUpdates.Add(
            new ComplaintUpdate
            {
                ComplaintId =
                    complaintId,

                UpdatedById =
                    updatedById,

                Note =
                    string.IsNullOrWhiteSpace(note)
                        ? $"Status changed to {status}."
                        : note,

                Timestamp =
                    DateTime.Now
            }
        );

        await _context.SaveChangesAsync();

        return true;
    }
}
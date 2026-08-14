using KnustHallMaintenance.Models;

namespace KnustHallMaintenance.Services;

public interface IComplaintService
{
    Task<Complaint> CreateAsync(Complaint complaint);

    Task<List<Complaint>> GetForStudentAsync(int studentId);

    Task<List<Complaint>> GetAllAsync(
        ComplaintStatus? statusFilter = null,
        ComplaintCategory? categoryFilter = null);

    Task<List<Complaint>> GetForHallAsync(
        string hall,
        ComplaintStatus? statusFilter = null,
        ComplaintCategory? categoryFilter = null);

    Task<List<Complaint>> GetAssignedToAsync(
        int maintenanceId,
        ComplaintStatus? statusFilter = null,
        ComplaintCategory? categoryFilter = null);

    Task<Complaint?> GetByIdAsync(int id);

    Task<bool> AssignAsync(
        int complaintId,
        int staffId,
        int assignToId);

    Task<bool> UpdateStatusAsync(
        int complaintId,
        ComplaintStatus status,
        string? note,
        int updatedById);
}
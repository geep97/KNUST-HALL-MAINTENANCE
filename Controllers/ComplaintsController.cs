using System.Security.Claims;
using KnustHallMaintenance.Models;
using KnustHallMaintenance.Models.ViewModels;
using KnustHallMaintenance.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KnustHallMaintenance.Controllers;

[Authorize(Roles = "Student")]
public class ComplaintsController : Controller
{
    private readonly IComplaintService _service;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private const long MaxPhotoSize = 5 * 1024 * 1024;

    public ComplaintsController(
        IComplaintService service,
        IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
    }

    private int CurrentUserId =>
        int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

    private string CurrentStudentHall =>
        User.FindFirstValue("Hall") ?? string.Empty;

    private string CurrentStudentRoom =>
        User.FindFirstValue("RoomNumber") ?? string.Empty;

    public async Task<IActionResult> MyComplaints()
    {
        var complaints =
            await _service.GetForStudentAsync(CurrentUserId);

        return View(complaints);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new ComplaintCreateViewModel
        {
            Hall = CurrentStudentHall,
            RoomNumber = CurrentStudentRoom
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ComplaintCreateViewModel model)
    {
        try
        {
            var hall = CurrentStudentHall;
            var roomNumber = CurrentStudentRoom;

            model.Hall = hall;
            model.RoomNumber = roomNumber;

            IFormFile? uploadedPhoto = null;

            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                uploadedPhoto = form.Files["Photo"];
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? photoPath = null;

            if (uploadedPhoto != null &&
                uploadedPhoto.Length > 0)
            {
                if (uploadedPhoto.Length > MaxPhotoSize)
                {
                    ModelState.AddModelError(
                        "Photo",
                        "The photo must be 5 MB or smaller.");

                    return View(model);
                }

                var extension =
                    Path.GetExtension(
                        uploadedPhoto.FileName)
                    .ToLowerInvariant();

                if (!AllowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "Photo",
                        "Only JPG, JPEG, PNG and WEBP images are allowed.");

                    return View(model);
                }

                var uploadsDirectory =
                    Path.Combine(
                        _env.WebRootPath,
                        "uploads");

                Directory.CreateDirectory(
                    uploadsDirectory);

                var fileName =
                    $"{Guid.NewGuid():N}{extension}";

                var filePath =
                    Path.Combine(
                        uploadsDirectory,
                        fileName);

                await using (
                    var stream = new FileStream(
                        filePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                {
                    await uploadedPhoto.CopyToAsync(stream);
                }

                photoPath =
                    $"/uploads/{fileName}";
            }

            var complaint = new Complaint
            {
                StudentId = CurrentUserId,
                Category = model.Category,
                Description = model.Description,
                Hall = hall,
                RoomNumber = roomNumber,
                PhotoPath = photoPath
            };

            await _service.CreateAsync(complaint);

            return RedirectToAction(
                nameof(MyComplaints));
        }
        catch
        {
            ModelState.AddModelError(
                "",
                "There was a problem submitting the request.");

            model.Hall = CurrentStudentHall;
            model.RoomNumber = CurrentStudentRoom;

            return View(model);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var complaint =
            await _service.GetByIdAsync(id);

        if (complaint == null ||
            complaint.StudentId != CurrentUserId)
        {
            return NotFound();
        }

        return View(complaint);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmResolution(
        int complaintId,
        bool confirmed,
        string? note)
    {
        var success =
            await _service.ConfirmResolutionAsync(
                complaintId,
                CurrentUserId,
                confirmed,
                note);

        if (!success)
        {
            return NotFound();
        }

        TempData["Success"] = confirmed
            ? "Thank you. The maintenance request has been confirmed as resolved."
            : "The request has been reopened and sent back to the maintenance team.";

        return RedirectToAction(
            nameof(Details),
            new { id = complaintId });
    }
}
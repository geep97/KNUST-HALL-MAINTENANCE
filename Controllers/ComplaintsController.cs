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
    private const int MaxPhotoCount = 5;

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
        var savedFiles = new List<string>();

        try
        {
            var hall = CurrentStudentHall;
            var roomNumber = CurrentStudentRoom;

            model.Hall = hall;
            model.RoomNumber = roomNumber;

            var uploadedPhotos = new List<IFormFile>();

            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();

                uploadedPhotos =
                    form.Files
                        .Where(f =>
                            string.Equals(
                                f.Name,
                                "Photos",
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }

            if (uploadedPhotos.Count > MaxPhotoCount)
            {
                ModelState.AddModelError(
                    "Photos",
                    $"You can upload a maximum of {MaxPhotoCount} photos."
                );
            }

            foreach (var photo in uploadedPhotos)
            {
                if (photo.Length <= 0)
                {
                    ModelState.AddModelError(
                        "Photos",
                        "One of the selected files is empty."
                    );

                    continue;
                }

                if (photo.Length > MaxPhotoSize)
                {
                    ModelState.AddModelError(
                        "Photos",
                        $"{photo.FileName} is larger than 5 MB."
                    );

                    continue;
                }

                var extension =
                    Path.GetExtension(photo.FileName)
                        .ToLowerInvariant();

                if (!AllowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "Photos",
                        $"{photo.FileName} is not a supported image type."
                    );
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var uploadsDirectory =
                Path.Combine(
                    _env.WebRootPath,
                    "uploads"
                );

            Directory.CreateDirectory(uploadsDirectory);

            var photoPaths = new List<string>();

            foreach (var photo in uploadedPhotos)
            {
                var extension =
                    Path.GetExtension(photo.FileName)
                        .ToLowerInvariant();

                var fileName =
                    $"{Guid.NewGuid():N}{extension}";

                var filePath =
                    Path.Combine(
                        uploadsDirectory,
                        fileName
                    );

                await using (
                    var stream = new FileStream(
                        filePath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                {
                    await photo.CopyToAsync(stream);
                }

                var photoPath =
                    $"/uploads/{fileName}";

                savedFiles.Add(filePath);
                photoPaths.Add(photoPath);
            }

            var complaint = new Complaint
            {
                StudentId = CurrentUserId,
                Category = model.Category,
                Description = model.Description,
                Hall = hall,
                RoomNumber = roomNumber,
                PhotoPath = photoPaths.FirstOrDefault()
            };

            foreach (var photoPath in photoPaths)
            {
                complaint.Photos.Add(
                    new ComplaintPhoto
                    {
                        PhotoPath = photoPath,
                        UploadedAt = DateTime.Now
                    }
                );
            }

            await _service.CreateAsync(complaint);

            return RedirectToAction(
                nameof(MyComplaints)
            );
        }
        catch
        {
            foreach (var filePath in savedFiles)
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch
                {
                }
            }

            ModelState.AddModelError(
                "",
                "There was a problem submitting the request."
            );

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
            new { id = complaintId }
        );
    }
}